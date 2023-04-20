using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New UI Ground", menuName = "Flowrest/UI Data")]
public class GroundUIData : ScriptableObject
{
   public string Name;
   public Sprite Icon;
   public Color ColorIcon;
   public Color ColorIconWhiter;
   // public int NbLeft;
   public AllStates GroundState;
}
