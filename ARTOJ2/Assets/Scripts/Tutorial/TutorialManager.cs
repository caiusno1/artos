using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public Canvas CounterCanvas;
    public TextMeshProUGUI CounterText;
    public int BigCounter;
    public int SmallCounter = 3;
    // Start is called before the first frame update
    void Start()
    {
        CounterText.text = "   " + SmallCounter;
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
}
