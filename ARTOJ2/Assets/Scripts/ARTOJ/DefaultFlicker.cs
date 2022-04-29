
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DefaultFlicker : MonoBehaviour
{
    private bool enabled = false;
    private int SOA_IN_FRAMES = 0;
    private int FrameIdx = 0;
    private UnityAction callback;
    private GameObject reference;
    private GameObject probe;
    private Image probeHidable;
    private Image refHidable;
    private int POSITIVE_SOA_IN_FRAMES = 0;
    private bool tojStarted = false;
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
                    probeHidable.enabled = false;
                }
                else if (SOA_IN_FRAMES > 0)
                {
                    refHidable.enabled = false;
                }
                else
                {
                    probeHidable.enabled = false;
                    refHidable.enabled = false;
                }
            }
            else if (FrameIdx == 1)
            {
                if (SOA_IN_FRAMES < 0)
                {
                    probeHidable.enabled = true;
                }
                else if (SOA_IN_FRAMES > 0)
                {
                    refHidable.enabled = true;
                }
                else
                {
                    probeHidable.enabled = true;
                    refHidable.enabled = true;
                    enabled = false;
                    this.callback.Invoke();
                }
            }


            if (FrameIdx == POSITIVE_SOA_IN_FRAMES)
            {
                if (SOA_IN_FRAMES < 0)
                {
                    refHidable.enabled = false;
                }
                else if (SOA_IN_FRAMES > 0)
                {
                    probeHidable.enabled = false;
                }
            }
            else if(FrameIdx == POSITIVE_SOA_IN_FRAMES + 1)
            {
                if (SOA_IN_FRAMES < 0)
                {
                    refHidable.enabled = true;
                }
                else if (SOA_IN_FRAMES > 0)
                {
                    probeHidable.enabled = true ;
                    
                }
                reference = null;
                probe = null;
                enabled = false;
                probeHidable = null;
                refHidable = null;
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
    private Image GetHideable(GameObject go)
    {
        return go.GetComponent<Image>();
    }
    public bool HasFinishedTOJ()
    {
        return !this.enabled;
    }
}
