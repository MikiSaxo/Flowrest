using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Niveau X", menuName = "Flowrest/Level Data")]
public class LevelData : ScriptableObject
{
   public string LevelName;
   public string LevelFolder;
   
   
   public int EnergyAtStart;

   
   public bool HasInventory;
   public AllStates StartNbState;
   public int[] StartNbAllState;
   
   public bool HasRecycling;
   public bool HasInfinitRecycling;
   public int NbOfRecycling;
   
   public bool OpenMemo;
   public bool HasPrevisu;
   public bool BlockLastSwap;

   public bool IsTuto;
   public bool IsTutoRecycling;
   public Vector2Int[] PlayerForceSwap;
   public bool HasForcePoseBlocAfterSwap;
   public Vector2Int ForcePoseBlocCoord;
   public PopUpInfos[] PopUpInfos;
   
   public string QuestDescription;
   public string QuestDescriptionEnglish;
   
   
   public AllStates[] QuestFloor;
   public AllStates[] QuestFlower;
   public AllStates[] QuestNoSpecificTiles;
   public AllStates[] QuestTileChain;
   public int NumberTileChain;
   public AllStates[] QuestTileCount;
   public int NumberTileCount;

   
   public DialogData DialogLevelStart;
   // public DialogData DialogLevelEnd;
   public string CharacterName;
   public Sprite[] CharacterSpritesBeginning;
   public Sprite[] CharacterSpritesEnd;
   public string[] DialogBeginning;
   public string[] DialogEnd;
   
   public string[] DialogBeginningEnglish;
   public string[] DialogEndEnglish;
}
