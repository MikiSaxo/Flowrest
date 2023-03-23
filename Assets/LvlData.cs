using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LvlData
{
    //[Header("Level")] public string LevelName;
   
    [Header("Crystal Data")]
    //[Tooltip("Values are like 50, 100, 550 -> MAX = 1000")] public int EnergyAtStart;
    public List<Vector2Int> Coords;
    //
    // [Header("Mechanics")] 
    // public bool HasInventory;
    // public bool HasTrashCan;
    // public bool BlockLastGroundsSwapped;
    //
    // [Header("Quests Info")]
    // public string QuestDescription;
    // public Sprite QuestImage;
    // [Header("Quests Choose")]
    // public AllStates[] WhichStateFloor;
    // public AllStates[] WhichStateFlower;
    // public AllStates[] WhichStateNoSpecificTiles;
    //
    // [Header("Dialogs")] public string[] DialogToDisplayAtTheBeginning;
    // public string[] DialogToDisplayAtTheEnd;
}
