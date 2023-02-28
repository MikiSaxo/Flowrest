using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New UI Ground", menuName = "Flowrest/UI Datas")]
public class GroundUIData : ScriptableObject
{
   public string Name;
   public Sprite Icon;
   public Color ColorIcon;
   public AllStates GroundState;
}
