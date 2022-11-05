using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaterData", menuName = "Water/Water Data")]
public class WaterData : ScriptableObject
{
    public char WaterName;
    public bool[] DirectionsNSWE = new bool[4];
}
