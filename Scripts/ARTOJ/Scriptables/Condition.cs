using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExperimetalCondition", menuName = "ARTOJ/ExperimetalCondition", order = 1)]
public class Condition : ScriptableObject
{
    public string Name;
    public List<float> values;
}
