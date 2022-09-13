using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


#if VUFORIA_ENABLED
    using Vuforia;
#endif



public class TOJGrid : MonoBehaviour
{
#if VUFORIA_ENABLED
    public ImageTargetBehaviour target;
#endif
    // public bool TOJ_ready = false;
    public Sprite defaultBackPlate;
    [HideInInspector]
    public bool StateSet = false;
    [HideInInspector]
    public bool LeftFirst = true;
    public bool probeIsLeft;
    public ButtonProxy leftBtn;
    public ButtonProxy rightBtn;

    public GameObject stimuliHolder;
    public int xaxisLength = 0;
    public int yaxisLength = 0;

    private DefaultFlicker flicker;

    public List<List<GameObject>> stimuli = new List<List<GameObject>>();

    private List<List<Sprite>> memoryColor = new List<List<Sprite>>();

    private List<List<int>> memoryLable = new List<List<int>>();

    public List<Sprite> colorsInput;

    public List<Sprite> colors;

    // public bool memoryTurnNow = false;
    // public bool firstCard = true;
    // public bool memoryTurnFinished = false;
    public GameObject firstCardGO;
    public GameObject secondCardGO;

    private string firstCardID;
    private string secondCardID;

    private int closedCards = 24;


    void Start()
    {
        colors = new List<Sprite>();
        colors.AddRange(colorsInput);
        colors.AddRange(colors);
        colors.Shuffle();
        flicker = GetComponent<DefaultFlicker>();
        this.StateSet = false;
        int idx = 0;
        int idx2 = 0;
        stimuli = new List<List<GameObject>>();
        stimuli.Add(new List<GameObject>());
        memoryColor = new List<List<Sprite>>();
        memoryLable = new List<List<int>>();
        memoryColor.Add(new List<Sprite>());
        memoryLable.Add(new List<int>());
        var iteratorVar = 0;
        foreach (Transform child in this.stimuliHolder.transform)
        {
            stimuli[idx2].Add(child.gameObject);
            var color = colors[iteratorVar];
            memoryColor[idx2].Add(color);
            child.gameObject.name = "" + colorsInput.IndexOf(color);
            memoryLable[idx2].Add(colorsInput.IndexOf(color));
            var tojGrid = this;
            var btnProxy = child.GetComponent<ButtonProxy>();
            btnProxy.AddListener(new ButtonMemoryAction(child,this,color, child.gameObject.name).getListener);
            idx++;
            if(idx >= xaxisLength)
            {
                idx = 0;
                idx2++;
                stimuli.Add(new List<GameObject>());
                memoryColor.Add(new List<Sprite>());
                memoryLable.Add(new List<int>());
            }
            iteratorVar++;
        }
    }    
    public IEnumerator StartSingleExperiment(Dictionary<string,object> condition, UnityAction InputEnabledCallback)
    {
        int soa = 0;
        if(condition["SOA"] is float csoa)
        {
            soa = (int)csoa;
        }
#if VUFORIA_ENABLED
     yield return new WaitWhile(() => target.TargetStatus.Equals(TargetStatus.NotObserved));
#else
    yield return new WaitWhile(() => ExperimentController.GetInstance().state != StateMachine.TOJ_READY);
#endif
        for (var i = 0; i< xaxisLength; i++)
        {
            for (var j = 0; j < yaxisLength; j++)
            {
                stimuli[j][i].SetActive(true);
            }
        }
        yield return new WaitForSecondsRealtime(1);
        var probeX = UnityEngine.Random.Range(0, xaxisLength - 1);
        var probeY = UnityEngine.Random.Range(0, yaxisLength - 1);

        int refX;
        int refY;
        do
        {
            refX = UnityEngine.Random.Range(0, xaxisLength - 1);
            refY = UnityEngine.Random.Range(0, yaxisLength - 1);
        }
        while (refX == probeX || memoryLable[refY][refX] == memoryLable[probeY][probeX]);

        //experimentalManipulation(probeX, probeY, refX, refY);

        this.probeIsLeft = probeX < refX;

        Debug.Log("PROBE LEFT:" + probeIsLeft);

        Debug.Log("PROBE X:" + probeX);
        Debug.Log("REF X:" + refX);

        Debug.Log("Bool:" + (refX < probeX));
        Debug.Log("SOA:" + soa);

        if ((refX > probeX && soa >= 0) || (refX < probeX && soa <= 0))
        {
            this.LeftFirst = false;
            this.StateSet = true;
            Debug.Log("Left last");
        }
        else
        {
            this.LeftFirst = true;
            this.StateSet = true;
            Debug.Log("Right last");
        }
        yield return new WaitForSecondsRealtime(1);
        flicker.callTOJ(soa, stimuli[probeY][probeX], stimuli[refY][refX], () => {
            InputEnabledCallback.Invoke();
        });
    }
    private IEnumerator ExecuteWholeExperiment(List<Dictionary<string,object>> conditions)
    {
        foreach(var cond in conditions)
        {
            yield return StartSingleExperiment(cond, () => { });
        }
    }
    private void experimentalManipulation(int probX, int probY, int refX, int refY)
    {
        var angle = UnityEngine.Random.Range(0, 359);
        foreach (var stimuliRow in stimuli)
        {
            foreach (var singleStimulus in stimuliRow)
            {
                singleStimulus.transform.rotation = Quaternion.identity;
                singleStimulus.transform.rotation *= Quaternion.AngleAxis(angle, singleStimulus.transform.forward);
            }
        }
        stimuli[probY][probX].transform.rotation = Quaternion.identity;
        stimuli[probY][probX].transform.rotation *= Quaternion.AngleAxis(angle+90, stimuli[probY][probX].transform.forward);
        stimuli[refY][refX].transform.rotation = Quaternion.identity;
        stimuli[refY][refX].transform.rotation *= Quaternion.AngleAxis(angle+90, stimuli[refY][refX].transform.forward);
    }

    // Update is called once per frame
    void Update()
    {
        if(ExperimentController.GetInstance().state == StateMachine.MemoryEvaluate)
        {
            StartCoroutine(ShowCardFor2Seconds(this));
        }
    }

    public IEnumerator ShowCardFor2Seconds(TOJGrid tojGrid)
    {
        if (ExperimentController.GetInstance().state == StateMachine.MemoryEvaluate)
        {
            var currentTrial = ExperimentController.GetInstance().trialLog[ExperimentController.GetInstance().trialLog.Count - 1];
            ExperimentController.GetInstance().state = StateMachine.MemoryDelayBeforeTOJ;
            currentTrial.firstSelectedPosition = tojGrid.firstCardGO.transform.GetSiblingIndex();
            currentTrial.secondSelectedPosition = tojGrid.secondCardGO.transform.GetSiblingIndex();
            var board = new int[4][];
            board[0] = memoryLable[0].ToArray();
            board[1] = memoryLable[1].ToArray();
            board[2] = memoryLable[2].ToArray();
            board[3] = memoryLable[3].ToArray();
            currentTrial.boardPositions = board;
            // 3 miliseconds difference between planed soa and actual soa is an arbitrary threshold :)
            //currentTrial.valid = Math.Abs(Math.Abs(currentTrial.soa)*1/60 - currentTrial.soaDuration) == 0;
            if (!currentTrial.valid && currentTrial.mode != "tutorial")
            {
                ExperimentController.GetInstance().runtimeSetup.Add(ExperimentController.GetInstance().CurrentCondition);
            }
            ExperimentController.GetInstance().sendResult();
            yield return new WaitForSeconds(1);
            if (tojGrid.firstCardID.Equals(tojGrid.secondCardID))
            {
                tojGrid.firstCardGO.GetComponent<ButtonProxy>().RemoveAllListeners();
                tojGrid.secondCardGO.GetComponent<ButtonProxy>().RemoveAllListeners();
                closedCards--;
                closedCards--;
                if (closedCards <= 2)
                {
                    foreach(var stimulusRow in stimuli){
                        foreach(var stimulus in stimulusRow)
                        {
                            var stimulusMeshRenderer = stimulus.GetComponentInChildren<SpriteRenderer>();
                            stimulusMeshRenderer.sprite = defaultBackPlate;
                        }
                    }
                    closedCards = 24;
                    this.Start();
                }
                Debug.Log("Memory Correct");
            }
            else
            {
                var firstCardMeshRenderer = tojGrid.firstCardGO.GetComponentInChildren<SpriteRenderer>();
                var secondCardMeshRenderer = tojGrid.secondCardGO.GetComponentInChildren<SpriteRenderer>();
                firstCardMeshRenderer.sprite = defaultBackPlate;
                secondCardMeshRenderer.sprite = defaultBackPlate;
                Debug.Log("Memory Wrong");
            }
            tojGrid.secondCardGO = null;
            tojGrid.firstCardGO = null;
            tojGrid.firstCardID = null;
            tojGrid.secondCardID = null;
            ExperimentController.GetInstance().state = StateMachine.TOJ_READY;
            ExperimentController.GetInstance().NextExperiment();
        }
    }

    private class ButtonMemoryAction
    {
        private TOJGrid tOJGrid;
        private Transform btn;
        private Sprite color;
        private string lable;
        public ButtonMemoryAction(Transform btn, TOJGrid tojGrid, Sprite color , string lable)
        {
            this.tOJGrid = tojGrid;
            this.btn = btn;
            this.color = color;
            this.lable = lable;
        }
        public void getListener()
        {
            if(ExperimentController.GetInstance().state == StateMachine.MemoryChoseFirst)
            {
                this.tOJGrid.firstCardGO = btn.gameObject;
                this.tOJGrid.firstCardID = lable;
            }
            else if(ExperimentController.GetInstance().state == StateMachine.MemoryChooseSecond && btn.gameObject != this.tOJGrid.firstCardGO)
            {
                this.tOJGrid.secondCardGO = btn.gameObject;
                this.tOJGrid.secondCardID = lable;
            }
            else if (btn.gameObject == this.tOJGrid.firstCardGO || (ExperimentController.GetInstance().state != StateMachine.MemoryChooseSecond && ExperimentController.GetInstance().state != StateMachine.MemoryChoseFirst))
            {
                return;
            }

            if (ExperimentController.GetInstance().state == StateMachine.MemoryChoseFirst || ExperimentController.GetInstance().state == StateMachine.MemoryChooseSecond)
            {
                var spriteRenderer = btn.GetComponentInChildren<SpriteRenderer>();
                spriteRenderer.sprite = color;
//                spriteRenderer.enabled = false;
//                spriteRenderer.enabled = true;
            }

            if (ExperimentController.GetInstance().state == StateMachine.MemoryChoseFirst)
            {
                ExperimentController.GetInstance().state = StateMachine.MemoryChooseSecond;
            }
            else
            {
                ExperimentController.GetInstance().state = StateMachine.MemoryEvaluate;
            }
        }
    }
}
