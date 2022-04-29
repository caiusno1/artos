using System;
using System.Collections;
using System.Collections.Generic;
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
    [HideInInspector]
    public bool StateSet = false;
    [HideInInspector]
    public bool LeftFirst = true;
    public bool probeIsLeft;
    public Button leftBtn;
    public Button rightBtn;

    public GameObject stimuliHolder;
    public int xaxisLength = 0;
    public int yaxisLength = 0;

    private DefaultFlicker flicker;

    public List<List<GameObject>> stimuli = new List<List<GameObject>>();

    private List<List<Material>> memoryColor = new List<List<Material>>();

    private List<List<string>> memoryLable = new List<List<string>>();

    public List<Material> colorsInput;

    public List<Material> colors;

    // public bool memoryTurnNow = false;
    // public bool firstCard = true;
    // public bool memoryTurnFinished = false;
    public GameObject firstCardGO;
    public GameObject secondCardGO;

    private string firstCardID;
    private string secondCardID;


    void Start()
    {
        colors = new List<Material>();
        colors.AddRange(colorsInput);
        colors.AddRange(colors);
        colors.Shuffle();
        flicker = GetComponent<DefaultFlicker>();
        this.StateSet = false;
        int idx = 0;
        int idx2 = 0;
        stimuli = new List<List<GameObject>>();
        stimuli.Add(new List<GameObject>());
        memoryColor.Add(new List<Material>());
        memoryLable.Add(new List<string>());
        var iteratorVar = 0;
        foreach (Transform child in this.stimuliHolder.transform)
        {
            stimuli[idx2].Add(child.gameObject);
            var color = colors[iteratorVar];
            memoryColor[idx2].Add(color);
            child.gameObject.name = "" + colorsInput.IndexOf(color);
            memoryLable[idx2].Add("" + iteratorVar);
            var tojGrid = this;
            child.GetComponent<Button>().onClick.AddListener(new ButtonMemoryAction(child,this,color, child.gameObject.name).getListener);
            idx++;
            if(idx >= xaxisLength)
            {
                idx = 0;
                idx2++;
                stimuli.Add(new List<GameObject>());
                memoryColor.Add(new List<Material>());
                memoryLable.Add(new List<string>());
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
        while (refX == probeX);

        experimentalManipulation(probeX, probeY, refX, refY);

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
        if(ExperimentController.GetInstance().state == StateMachine.MemoryEvaluate)
        {
            ExperimentController.GetInstance().state = StateMachine.MemoryDelayBeforeTOJ;
            yield return new WaitForSeconds(1);
            if (tojGrid.firstCardID.Equals(tojGrid.secondCardID))
            {
                tojGrid.firstCardGO.GetComponent<Button>().onClick.RemoveAllListeners();
                tojGrid.secondCardGO.GetComponent<Button>().onClick.RemoveAllListeners();
                Debug.Log("Memory Correct");
            }
            else
            {
                tojGrid.firstCardGO.GetComponent<Image>().material = null;
                tojGrid.secondCardGO.GetComponent<Image>().material = null;

                tojGrid.firstCardGO.GetComponent<Image>().enabled = false;
                tojGrid.firstCardGO.GetComponent<Image>().enabled = true;
                tojGrid.secondCardGO.GetComponent<Image>().enabled = false;
                tojGrid.secondCardGO.GetComponent<Image>().enabled = true;
                Debug.Log("Memory Wrong");
            }
            tojGrid.secondCardGO = null;
            tojGrid.firstCardGO = null;
            tojGrid.firstCardID = null;
            tojGrid.secondCardID = null;
            ExperimentController.GetInstance().state = StateMachine.TOJ_READY;
        }
    }

    private class ButtonMemoryAction
    {
        private TOJGrid tOJGrid;
        private Transform btn;
        private Material color;
        private string lable;
        public ButtonMemoryAction(Transform btn, TOJGrid tojGrid, Material color , string lable)
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
            else if (btn.gameObject == this.tOJGrid.firstCardGO)
            {
                return;
            }

                if (ExperimentController.GetInstance().state == StateMachine.MemoryChoseFirst || ExperimentController.GetInstance().state == StateMachine.MemoryChooseSecond)
            {
                btn.GetComponent<Image>().material = color;
                btn.GetComponent<Image>().enabled = false;
                btn.GetComponent<Image>().enabled = true;
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
