using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StateMachineSimulator : MonoBehaviour
{
    public bool shouldSimulate = true;
    private ExperimentController ctrl;
    public TOJGrid grid;
    private int gridi = 0;
    private int gridj = 0;
    private int Counter = 0;
    public TextMeshProUGUI FramerateMeasurementTool;
    // Start is called before the first frame update
    private bool allCleanedUp = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(shouldSimulate)
        {
            if(ctrl == null)
            {
                ctrl = ExperimentController.GetInstance();
            }
            if(ctrl.state == StateMachine.TOJ_EVAL)
            {
                ctrl.LeftBtnHandler();
            }
            else if (ctrl.state == StateMachine.MemoryChoseFirst)
            {
                Debug.Log(gridi + "" + gridj);
                grid.stimuli[gridi][gridj].GetComponent<Button>().onClick.Invoke();
                if(gridi >= grid.stimuli.Count - 2)
                {
                        if (gridj >= grid.stimuli[0].Count - 2)
                        {
                            gridi = 0;
                            gridj = 0;
                        }
                        else
                        {
                            gridi = 0;
                            gridj = gridj + 1;
                        }
                }
                else
                {
                    gridi = gridi + 1;
                }
            }
            else if (ctrl.state == StateMachine.MemoryChooseSecond)
            {
                grid.stimuli[gridi][gridj].GetComponent<Button>().onClick.Invoke();
                if (gridi >= grid.stimuli.Count - 2)
                {
                    if (gridj >= grid.stimuli[0].Count - 2)
                    {
                            gridi = 0;
                            gridj = 0;
                    }
                    else
                    {
                        gridi = 0;
                        gridj = gridj + 1;
                    }
                }
                else
                {
                    gridi = gridi + 1;
                }
            }
            Counter += 1;
            if(Counter > 6000)
            {
                Counter = 0;
            }
            FramerateMeasurementTool.text = ""+Counter;
        }
        else
        {
            if (!allCleanedUp)
            {
                if (FramerateMeasurementTool != null && FramerateMeasurementTool.gameObject != null)
                {
                    Destroy(FramerateMeasurementTool.transform.parent.gameObject);
                }
                allCleanedUp = true;
            }
        }
        
    }
}
