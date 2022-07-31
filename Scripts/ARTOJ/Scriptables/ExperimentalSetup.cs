using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ExperimentalSetup", menuName = "ARTOJ/ExperimentalSetup", order = 1)]
public class ExperimentalSetup : ScriptableObject
{
    public string Name;
    public List<Condition> conditions;
}
