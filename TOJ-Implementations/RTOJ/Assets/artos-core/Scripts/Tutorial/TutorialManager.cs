using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public Canvas CounterCanvas;
    public TextMeshProUGUI CounterText;
    public GameObject startBtn;
    public int BigCounter;
    public int SmallCounter = 3;
    public GameObject TabletBtns;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(ExperimentController.GetInstance().state == StateMachine.Tutorial)
        {
            if (SmallCounter > 0)
            {
                if (BigCounter >= 60)
                {
                    SmallCounter--;
                    CounterText.text = "   " + SmallCounter;
                    BigCounter = 0;
                }
                this.BigCounter++;
            }
            else
            {
                ExperimentController.GetInstance().state = StateMachine.TOJ_READY;
                Destroy(CounterCanvas.gameObject);
                Destroy(this);
            }
        }

    }
    public void OnStartFired()
    {
        if(TabletBtns != null)
        {
            TabletBtns.SetActive(true);
        }
        CounterCanvas.gameObject.SetActive(true);
        Destroy(startBtn);
        CounterText.text = "   " + SmallCounter;
        ExperimentController.GetInstance().state = StateMachine.StartFired;
    }

    internal void SetParticipantName(string name, int iD)
    {
        if (startBtn.GetComponent<Button>())
        {
            startBtn.GetComponentsInChildren<TMP_Text>()[0].text = "Start Experiment (" + name + ";" + iD + ")";
        }
        else
        {
            startBtn.GetComponentsInChildren<TMP_Text>()[1].text = "Start Experiment (" + name + ";" + iD + ")";
        }

    }
}
