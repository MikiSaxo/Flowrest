using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crystal", menuName = "Flowrest/Crystals Level Datas")]
public class CrystalLevelData : ScriptableObject
{
   [Tooltip("Values are like 50, 100, 550 -> MAX = 1000")] public int EnergyAtStart;
   public Vector2Int[] Coords;
}
