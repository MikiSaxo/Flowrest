using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crystal", menuName = "Flowrest/Crystals Level Datas")]
public class LevelData : ScriptableObject
{
   public string LevelName;
   public string LevelFolder;
   
   [Tooltip("Values are like 50, 100, 550 -> MAX = 1000")] public int EnergyAtStart;

   public bool HasInventory;
   public bool HasTrashCan;
   public bool HasPrevisu;
   public bool BlockLastSwap;
   public Vector2Int[] PlayerForceChangeThese2Tiles;

   public string QuestDescription;
   public Sprite QuestImage;
   
   public AllStates[] QuestFloor;
   public AllStates[] QuestFlower;
   public AllStates[] QuestNoSpecificTiles;
   public AllStates[] QuestTileChain;
   public int NumberTileChain;
   public AllStates[] QuestTileCount;
   public int NumberTileCount;

   public string CharacterName;
   public string[] DialogBeginning;
   public string[] DialogEnd;
}
