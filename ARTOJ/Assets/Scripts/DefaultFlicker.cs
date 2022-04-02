using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DefaultFlicker : MonoBehaviour
{
    private bool enabled = false;
    private int SOA_IN_FRAMES = 0;
    private int FrameIdx = 0;
    private UnityAction callback;
    private GameObject reference;
    private GameObject probe;
    private int POSITIVE_SOA_IN_FRAMES = 0;
    private bool firstFrame = false;
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
                if (SOA_IN_FRAMES < 0)
                {
                    probe.SetActive(false);
                }
                else if (SOA_IN_FRAMES > 0)
                {
                    reference.SetActive(false);
                }
                else
                {
                    probe.SetActive(false);
                    reference.SetActive(false);
                }
            }
            else if (FrameIdx == 2)
            {
                if (SOA_IN_FRAMES < 0)
                {
                    probe.SetActive(true);
                }
                else if (SOA_IN_FRAMES > 0)
                {
                    reference.SetActive(true);
                }
                else
                {
                    probe.SetActive(true);
                    reference.SetActive(true);
                    enabled = false;
                    this.callback.Invoke();
                }
            }


            if (FrameIdx == POSITIVE_SOA_IN_FRAMES)
            {
                if(SOA_IN_FRAMES < 0)
                {
                    reference.SetActive(false);
                }
                else if (SOA_IN_FRAMES > 0)
                {
                    probe.SetActive(false);
                }
            }
            else if(FrameIdx == POSITIVE_SOA_IN_FRAMES + 2)
            {
                if (SOA_IN_FRAMES < 0)
                {
                    reference.SetActive(true);
                }
                else if (SOA_IN_FRAMES > 0)
                {
                    probe.SetActive(true);
                }
                enabled = false;
                this.callback.Invoke();
            }
            this.FrameIdx++;
        }
    }
    public void callTOJ(int SOA_IN_FRAMES, GameObject probe, GameObject reference, UnityAction callback)
    {
        this.SOA_IN_FRAMES = SOA_IN_FRAMES;
        this.probe = probe;
        this.reference = reference;
        this.FrameIdx = 0;
        this.POSITIVE_SOA_IN_FRAMES = Mathf.Abs(SOA_IN_FRAMES);
        this.callback = callback;
        this.enabled = true;
    }
    public bool HasFinishedTOJ()
    {
        return this.enabled;
    }
}
