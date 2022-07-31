using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateMachine
{
    Preparation,
    WaitForStart,
    StartFired,
    Tutorial,
    TOJ_READY,
    TOJ_EVAL,
    MemoryChoseFirst,
    MemoryChooseSecond,
    MemoryEvaluate,
    MemoryDelayBeforeTOJ,
    TutorialFinished,
    ExperimentFinished,
}
