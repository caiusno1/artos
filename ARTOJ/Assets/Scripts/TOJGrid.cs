using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class TOJGrid : MonoBehaviour
{
    public GameObject stimulus;
    public int gridDim = 2;
    public float distanceBetween = 0.5f;
    public float timeInBetween = 0.020f;
    public int repetitions = 2;
    public List<float> soas = new List<float>();
    public ImageTargetBehaviour target;
    [HideInInspector]
    public bool StateSet = false;
    [HideInInspector]
    public bool LeftFirst = true;
    public bool probeIsLeft;
    public Button leftBtn;
    public Button rightBtn;

    private DefaultFlicker flicker;

    private List<List<GameObject>> stimuli = new List<List<GameObject>>();


    void Awake()
    {
        flicker = GetComponent<DefaultFlicker>();
        this.StateSet = false;
        var shift = gridDim / 2;
        for(var i=0; i<gridDim; i++)
        {
            stimuli.Add(new List<GameObject>());
            for (var j = 0; j < gridDim; j++)
            {
                var singleElement = Instantiate(stimulus, transform);
                singleElement.SetActive(false);
                singleElement.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                var pos = new Vector3(-shift + (i * distanceBetween), -shift + (j * distanceBetween), 0);
                singleElement.transform.localPosition = pos;
                stimuli[i].Add(singleElement);
            }
        }

    }    
    public IEnumerator StartSingleExperiment(Dictionary<string,object> condition)
    {
        int soa = 0;
        if(condition["SOA"] is float csoa)
        {
            soa = (int)csoa;
        }

        yield return new WaitWhile(() => target.TargetStatus.Equals(TargetStatus.NotObserved));
        for(var i = 0; i< gridDim; i++)
        {
            for (var j = 0; j < gridDim; j++)
            {
                stimuli[i][j].SetActive(true);
            }
        }
        yield return new WaitForSecondsRealtime(1);
        var probeX = UnityEngine.Random.Range(0, gridDim - 1);
        var probeY = UnityEngine.Random.Range(0, gridDim - 1);
        int refX;
        int refY;
        do
        {
            refX = UnityEngine.Random.Range(0, gridDim - 1);
            refY = UnityEngine.Random.Range(0, gridDim - 1);
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
        flicker.callTOJ(soa, stimuli[probeX][probeY], stimuli[refX][refY], () => { });
    }
    private IEnumerator ExecuteWholeExperiment(List<Dictionary<string,object>> conditions)
    {
        foreach(var cond in conditions)
        {
            yield return StartSingleExperiment(cond);
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
        stimuli[probX][probY].transform.rotation = Quaternion.identity;
        stimuli[probX][probY].transform.rotation *= Quaternion.AngleAxis(angle+90, stimuli[probX][probY].transform.forward);
        stimuli[refX][refY].transform.rotation = Quaternion.identity;
        stimuli[refX][refY].transform.rotation *= Quaternion.AngleAxis(angle+90, stimuli[refX][refY].transform.forward);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
