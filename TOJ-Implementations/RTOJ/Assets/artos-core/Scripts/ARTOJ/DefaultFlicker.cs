
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
    private float SOAEndTime = -1;
    private int framerate = 60;
    private Dictionary<GameObject, GameObject> objectCache = new Dictionary<GameObject, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        this.framerate = ExperimentController.GetInstance().FrameRate;
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
                this.SOAStartTime = Time.time;
            }
            else if (FrameIdx == 2)
            {
                this.soaDuration = Time.deltaTime;
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
            }

            if(FrameIdx > 1 && FrameIdx <= POSITIVE_SOA_IN_FRAMES)
            {
                this.soaDuration += Time.deltaTime;
            }

            if (FrameIdx == POSITIVE_SOA_IN_FRAMES)
            {
                if (SOA_IN_FRAMES < 0)
                {
                    refHidable.active = false;
                }
                else if (SOA_IN_FRAMES > 0)
                {
                    probeHidable.active = false;
                }
                this.SOAEndTime = Time.time;
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
                Debug.Log(this.soaDuration);
                var currTrial = ExperimentController.GetInstance().GetCurrentTrial();
                currTrial.SOAStartTime = this.SOAStartTime;
                currTrial.SOAEndTime = this.SOAEndTime;
                currTrial.soaDuration = this.soaDuration;
                currTrial.realSOA = this.SOAEndTime - this.SOAStartTime;
                currTrial.valid = !invalid;
                Debug.Log(currTrial);
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
        if (objectCache.ContainsKey(go))
        {
            return objectCache[go];
        }
        else
        {
            if (inR)
            {
                var retVal = go.GetComponent<Image>().gameObject;
                objectCache.Add(go, retVal);
                return retVal;
            }
            else
            {
                var retVal = go.GetComponentInChildren<SpriteRenderer>().gameObject;
                objectCache.Add(go, retVal);
                return retVal;
            }
        }


    }
    public bool HasFinishedTOJ()
    {
        return !this.enabled;
    }
}
