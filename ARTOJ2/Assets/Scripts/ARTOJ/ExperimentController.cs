using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ExperimentController : MonoBehaviour
{
    public ExperimentalSetup currentSetup;
    public List<Dictionary<string, object>> runtimeSetup;
    public List<GameObject> ExperimentalPositions;
    public bool shuffelOrder = false;
    public AudioClip correctSound;
    public AudioClip wrongSound;
    private int currentPosition;
    private int repetitions = 3;
    private static ExperimentController Instance;
    private Button leftBtn;
    private Button rightBtn;
    private int currentState = 0;
    public TMPro.TMP_Text stateField;
    private List<TOJResult> results = new List<TOJResult>();
    private int timeStamp = 0;
    private bool tutorialMode = true;
    //private bool InputEnabled = false;
    public StateMachine state = StateMachine.Preparation;
    private void Awake()
    {
        if(Instance == null)
        {
            QualitySettings.vSyncCount = 1;

            Application.targetFrameRate = 60;
            // Taken from here https://stackoverflow.com/questions/17994935/how-to-get-unix-time-stamp-in-net
            timeStamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
            // TODO create new runtimeSetup
            this.runtimeSetup = new List<Dictionary<string, object>>();

            if(this.currentSetup.conditions.Count <= 0)
            {
                throw new System.Exception("No conditions are provided");
            }
            else if(this.currentSetup.conditions.Count == 1)
            {

                System.Random r = new System.Random();
                for (var i = 0; i < 10; i++)
                {
                    this.runtimeSetup.Add(new Dictionary<string, object>());
                    this.runtimeSetup[this.runtimeSetup.Count - 1].Add(this.currentSetup.conditions[0].Name, this.currentSetup.conditions[0].values[r.Next(0, this.currentSetup.conditions[0].values.Count-1)]);
                    this.runtimeSetup[this.runtimeSetup.Count - 1].Add("Mode", "Tutorial");
                }
                for(var repIdx = 0; repIdx < repetitions; repIdx++)
                {
                    this.currentSetup.conditions.Shuffle();
                    for (var i = 0; i < this.currentSetup.conditions[0].values.Count; i++)
                    {
                        this.runtimeSetup.Add(new Dictionary<string, object>());
                        this.runtimeSetup[this.runtimeSetup.Count - 1].Add(this.currentSetup.conditions[0].Name, this.currentSetup.conditions[0].values[i]);
                    }
                }

            }
            else if(this.currentSetup.conditions.Count > 1)
            {
                throw new NotImplementedException();
                // TODO implement tutorial for this branch
                var listOfLists = currentSetup.conditions.Select((x) => x.values);
                var product = Extensions.Cartesian(listOfLists);
                var initRound = true;
                foreach(var trial in product)
                {
                    var idx = 0;
                    foreach(var prop in trial)
                    {
                        if (initRound)
                        {
                            this.runtimeSetup.Add(new Dictionary<string, object>());
                        }
                        this.runtimeSetup[idx].Add(this.currentSetup.conditions[idx].Name, prop);
                        idx++;
                    }
                    initRound = false;
                }
            }

            // Dummy setup
            /*this.runtimeSetup = new List<Dictionary<string, object>>();
            
            runtimeSetup.Add(new Dictionary<string, object>());
            runtimeSetup[0].Add("SOA", -100);
            runtimeSetup.Add(new Dictionary<string, object>());
            runtimeSetup[1].Add("SOA", +100);*/

            ExperimentController.Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Source https://codereview.stackexchange.com/questions/122699/finding-a-cartesian-product-of-multiple-lists
    
    public static ExperimentController GetInstance()
    {
       
        if(Instance == null)
        {
            ExperimentController.Instance = new GameObject("ExperimentController").AddComponent<ExperimentController>();         
        }
        return Instance;
    }
    // Start is called before the first frame update
    void Start()
    {
        if(ExperimentalPositions.Count  > 0)
        {
            this.currentPosition = 0;
            ExperimentalPositions[this.currentPosition % ExperimentalPositions.Count].transform.GetChild(0).gameObject.SetActive(true);
            leftBtn = ExperimentalPositions[this.currentPosition % ExperimentalPositions.Count].GetComponentInChildren<TOJGrid>().leftBtn;
            rightBtn = ExperimentalPositions[this.currentPosition % ExperimentalPositions.Count].GetComponentInChildren<TOJGrid>().rightBtn;
            leftBtn.onClick.AddListener(LeftBtnHandler);
            rightBtn.onClick.AddListener(RightBtnHandler);
            //this.InputEnabled = false;
            this.state = StateMachine.Tutorial;
            StartCoroutine(ExperimentalPositions[this.currentPosition % ExperimentalPositions.Count].GetComponentInChildren<TOJGrid>().StartSingleExperiment(this.runtimeSetup[currentPosition], () => {
                //this.InputEnabled = true;
                this.state = StateMachine.TOJ_EVAL;
            }));
        }
    }
    void Update()
    {
        // TODO wait for finish of trial first
        if (Input.GetKeyDown(KeyCode.Q) && this.state == StateMachine.TOJ_EVAL)
        {
            LeftBtnHandler();
        }
        else if(Input.GetKeyDown(KeyCode.P) && this.state == StateMachine.TOJ_EVAL)
        {
            RightBtnHandler();
        }
    }
    public void LeftBtnHandler()
    {
        leftBtn.onClick.RemoveAllListeners();
        rightBtn.onClick.RemoveAllListeners();
        var result = CheckUserResult(true);
        var probeSelected = CheckProbeFirst(true);
        if(this.runtimeSetup[this.currentPosition].ContainsKey("Mode") && this.runtimeSetup[this.currentPosition]["Mode"] == "Tutorial")
        {
            Debug.Log("Tutorial");
        } 
        else
        {
            Debug.Log("Real Data");
            sendResult(result, result, probeSelected);
        }

        PlayFeedbackSound(result);
        applyPoints(result);
        if (currentPosition < runtimeSetup.Count - 1)
        {
            ExperimentController.GetInstance().state = StateMachine.MemoryChoseFirst;
            StartCoroutine(this.NextMemory());
        }
        else
        {
            ExperimentController.Instance.state = StateMachine.ExperimentFinished;
            Debug.Log("Experiment finished");
            Application.Quit();
        }
    }
    public void RightBtnHandler()
    {
        leftBtn.onClick.RemoveAllListeners();
        rightBtn.onClick.RemoveAllListeners();
        var result = CheckUserResult(false);
        var probeSelected = CheckProbeFirst(false);

        if (this.runtimeSetup[this.currentPosition].ContainsKey("Mode") && this.runtimeSetup[this.currentPosition]["Mode"] == "Tutorial")
        {
            Debug.Log("Tutorial");
        }
        else 
        {
            Debug.Log("Real Data");
            sendResult(!result, result, probeSelected);
        }

        PlayFeedbackSound(result);
        applyPoints(result);
        if (currentPosition < runtimeSetup.Count - 1)
        {
            ExperimentController.GetInstance().state = StateMachine.MemoryChoseFirst;
            StartCoroutine(this.NextMemory());
        }
        else
        {
            Debug.Log("Experiment finished");
            Application.Quit();
        }
    }
    private IEnumerator NextMemory()
    {
        yield return new WaitUntil(() => ExperimentController.GetInstance().state == StateMachine.TOJ_READY);
        this.NextExperiment();
    }
    public void NextExperiment()
    {
        if(this.currentPosition < runtimeSetup.Count - 1)
        {
            ExperimentalPositions[this.currentPosition % ExperimentalPositions.Count].transform.GetChild(0).gameObject.SetActive(false);
            this.currentPosition++;
            ExperimentalPositions[this.currentPosition % ExperimentalPositions.Count].transform.GetChild(0).gameObject.SetActive(true);
            leftBtn = ExperimentalPositions[this.currentPosition % ExperimentalPositions.Count].GetComponentInChildren<TOJGrid>().leftBtn;
            rightBtn = ExperimentalPositions[this.currentPosition % ExperimentalPositions.Count].GetComponentInChildren<TOJGrid>().rightBtn;
            StartCoroutine(ExperimentalPositions[this.currentPosition % ExperimentalPositions.Count].GetComponentInChildren<TOJGrid>().StartSingleExperiment(this.runtimeSetup[currentPosition], () => {
                this.state = StateMachine.TOJ_EVAL;
                leftBtn.onClick.AddListener(LeftBtnHandler);
                rightBtn.onClick.AddListener(RightBtnHandler);
            }));

        }
    }
    public void applyPoints(bool feedback)
    {
        if (feedback)
        {
            this.currentState += 50;
        }
        else
        {
            this.currentState -= 50;
        }
        if(this.stateField != null)
        {
            this.stateField.text = "" + this.currentState;
        }
        else
        {
            Debug.LogWarning("No Statfield defined");
        }

    }

    public void sendResult(bool leftLast, bool feedback, bool probeSelected)
    {
        var jsonContent = "{'leftLast':"+leftLast+",'feedback':"+feedback+", 'soa':'"+ this.runtimeSetup[currentPosition]["SOA"] + "', 'probeFirstSelected':'" + probeSelected + "'}";
        Debug.Log(jsonContent);
        var result = new TOJResult();
        result.id = 0;
        result.timestamp = this.timeStamp;
        result.result = jsonContent;
        results.Add(result);
        var jsonifiedList = results.Select((res) => JsonUtility.ToJson(res));
        UnityWebRequest www = UnityWebRequest.Put("http://localhost:8000/results/?code=dg4fk%24%23385g9%23%3B0j%C3%9Fh%23%23s(7d%23%230gfh", "["+ String.Join(",",jsonifiedList) +"]");
        www.SetRequestHeader("Content-Type", "application/json");
        var webReq = www.SendWebRequest();

        
    }

    private void ExperimentController_completed(UnityWebRequestAsyncOperation obj)
    {
        if (obj.webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(obj.webRequest.error);
        }
        else
        {
            Debug.Log("Data upload complete!");
        }
    }

    public bool CheckUserResult(bool LeftLast)
    {
        float soa = 0.1f;
        if (this.runtimeSetup[currentPosition]["SOA"] is float csoa)
        {
            soa = csoa;
        }
        return this.ExperimentalPositions[this.currentPosition % ExperimentalPositions.Count].transform.GetComponent<TOJGrid>().LeftFirst != LeftLast || soa == 0;
    }
    public bool CheckProbeFirst(bool leftSelected)
    {
        return this.ExperimentalPositions[this.currentPosition % ExperimentalPositions.Count].transform.GetComponent<TOJGrid>().probeIsLeft == leftSelected;
    }
    public void PlayFeedbackSound(bool feedback)
    {
        if (feedback)
        {
            GetComponent<AudioSource>().PlayOneShot(this.correctSound);
        }
        else
        {
            GetComponent<AudioSource>().PlayOneShot(this.wrongSound);
        }
    }
}

public static class Extensions
{
    private static System.Random rng = new System.Random();
    public static IEnumerable<IEnumerable> Cartesian(this IEnumerable<IEnumerable> items)
    {
        var slots = items
           // initialize enumerators
           .Select(x => x.GetEnumerator())
           // get only those that could start in case there is an empty collection
           .Where(x => x.MoveNext())
           .ToArray();

        while (true)
        {
            // yield current values
            yield return slots.Select(x => x.Current);

            // increase enumerators
            foreach (var slot in slots)
            {
                // reset the slot if it couldn't move next
                if (!slot.MoveNext())
                {
                    // stop when the last enumerator resets
                    if (slot == slots.Last()) { yield break; }
                    slot.Reset();
                    slot.MoveNext();
                    // move to the next enumerator if this reseted
                    continue;
                }
                // we could increase the current enumerator without reset so stop here
                break;
            }
        }
    }
    // https://stackoverflow.com/questions/273313/randomize-a-listt
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
