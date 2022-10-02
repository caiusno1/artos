
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR;

public class DefaultFlicker : MonoBehaviour
{
    public bool inR = false;
    private bool enabled = false;
    private int SOA_IN_FRAMES = 0;
    private int FrameIdx = 0;
    private UnityAction callback;
    private GameObject reference;
    private GameObject probe;
    private GameObject probeHidable;
    private GameObject refHidable;
    private int POSITIVE_SOA_IN_FRAMES = 0;
    private bool tojStarted = false;
    private float soaDuration = -1;
    private bool invalid = false;
    private float SOAStartTime = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enabled)
        {
            if (FrameIdx == 0)
            {
                this.soaDuration = 0;
                invalid = false;
                if (SOA_IN_FRAMES < 0)
                {
                    probeHidable.active = false;
                }
                else if (SOA_IN_FRAMES > 0)
                {
                    refHidable.active = false;
                }
                else
                {
                    probeHidable.active = false;
                    refHidable.active = false;
                }
                SOAStartTime = Time.time;
            }
            else if (FrameIdx == 2)
            {
                if (SOA_IN_FRAMES < 0)
                {
                    probeHidable.active = true;
                }
                else if (SOA_IN_FRAMES > 0)
                {
                    refHidable.active = true;
                }
                else
                {
                    probeHidable.active = true;
                    refHidable.active = true;
                    enabled = false;
                    this.callback.Invoke();
                }
                if(Time.time-SOAStartTime > 1 / 60)
                {
                    Debug.Log("Too Short!");
                    invalid = true;
                }
            }

            if (FrameIdx == POSITIVE_SOA_IN_FRAMES)
            {
                this.soaDuration = Time.time - SOAStartTime;
                if (SOA_IN_FRAMES < 0)
                {
                    refHidable.active = false;
                }
                else if (SOA_IN_FRAMES > 0)
                {
                    probeHidable.active = false;
                }
            }
            else if(FrameIdx == POSITIVE_SOA_IN_FRAMES + 2)
            {
                if (SOA_IN_FRAMES < 0)
                {
                    refHidable.active = true;
                }
                else if (SOA_IN_FRAMES > 0)
                {
                    probeHidable.active = true ;               
                }
                reference = null;
                probe = null;
                enabled = false;
                probeHidable = null;
                refHidable = null;
                SOAStartTime = -1;
                Debug.Log(this.soaDuration);
                ExperimentController.GetInstance().trialLog[ExperimentController.GetInstance().trialLog.Count - 1].soaDuration = this.soaDuration;
                ExperimentController.GetInstance().trialLog[ExperimentController.GetInstance().trialLog.Count - 1].realSOA = Mathf.CeilToInt(this.soaDuration / (1/60));
                ExperimentController.GetInstance().trialLog[ExperimentController.GetInstance().trialLog.Count - 1].valid = !invalid;
                Debug.Log(ExperimentController.GetInstance().trialLog[ExperimentController.GetInstance().trialLog.Count - 1]);
                this.callback.Invoke();
            }
            this.FrameIdx++;
        }
    }
    public void callTOJ(int SOA_IN_FRAMES, GameObject probe, GameObject reference, UnityAction callback)
    {
        this.SOA_IN_FRAMES = SOA_IN_FRAMES;
        this.probe = probe;
        this.probeHidable = GetHideable(probe);
        this.reference = reference;
        this.refHidable = GetHideable(reference);
        this.FrameIdx = 0;
        this.POSITIVE_SOA_IN_FRAMES = Mathf.Abs(SOA_IN_FRAMES);
        this.callback = callback;
        this.enabled = true;
    }
    private GameObject GetHideable(GameObject go)
    {
        if (inR)
        {
            return go.GetComponent<Image>().gameObject;
        }
        else
        {
            return go.GetComponentInChildren<SpriteRenderer>().gameObject;
        }

    }
    public bool HasFinishedTOJ()
    {
        return !this.enabled;
    }
}
