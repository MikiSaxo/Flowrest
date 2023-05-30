using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class TilesCondition
{
  public AllStates Current;
  public AllStates Other;
  public AllStates Result;
}
