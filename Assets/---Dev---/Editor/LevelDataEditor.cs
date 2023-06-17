using System;
using UnityEditor;
using UnityEngine;

[CustomEditor((typeof(LevelData)))]
public class LevelDataEditor : Editor
{
    private bool level;
    private bool energy;
    private bool mechanics;
    private bool questsInfo;
    private bool questsChoose;
    private bool dialogs;


    private SerializedProperty LevelName;
    private SerializedProperty LevelFolder;

    private SerializedProperty EnergyAtStart;

    private SerializedProperty HasInventory;
    private SerializedProperty StartNbState;
    private SerializedProperty StartNbAllState;
    private SerializedProperty HasRecycling;
    private SerializedProperty HasInfinitRecycling;
    private SerializedProperty NbOfRecycling;
    private SerializedProperty OpenMemo;
    private SerializedProperty HasPrevisu;
    private SerializedProperty BlockLastSwap;
    private SerializedProperty IsTuto;
    private SerializedProperty IsTutoRecycling;
    private SerializedProperty PlayerForceSwap;
    private SerializedProperty HasForcePoseBlocAfterSwap;
    private SerializedProperty ForcePoseBlocCoord;
    private SerializedProperty PopUpInfos;

    private SerializedProperty QuestDescription;
    private SerializedProperty QuestDescriptionEnglish;

    private SerializedProperty QuestFloor;
    private SerializedProperty QuestFlower;
    private SerializedProperty QuestNoSpecificTiles;
    private SerializedProperty QuestTileChain;
    private SerializedProperty NumberTileChain;
    private SerializedProperty QuestTileCount;
    private SerializedProperty NumberTileCount;

    private SerializedProperty DialogLevelStart;
    private SerializedProperty DialogLevelEnd;
    private SerializedProperty CharacterName;
    private SerializedProperty CharacterSpritesBeginning;
    private SerializedProperty CharacterSpritesEnd;
    private SerializedProperty DialogBeginning;
    private SerializedProperty DialogEnd;
    private SerializedProperty DialogBeginningEnglish;
    private SerializedProperty DialogEndEnglish;


    void OnEnable()
    {
        LevelName = serializedObject.FindProperty("LevelName");
        LevelFolder = serializedObject.FindProperty("LevelFolder");

        EnergyAtStart = serializedObject.FindProperty("EnergyAtStart");

        HasInventory = serializedObject.FindProperty("HasInventory");
        StartNbState = serializedObject.FindProperty("StartNbState");
        StartNbAllState = serializedObject.FindProperty("StartNbAllState");
        HasRecycling = serializedObject.FindProperty("HasRecycling");
        HasInfinitRecycling = serializedObject.FindProperty("HasInfinitRecycling");
        NbOfRecycling = serializedObject.FindProperty("NbOfRecycling");
        OpenMemo = serializedObject.FindProperty("OpenMemo");
        HasPrevisu = serializedObject.FindProperty("HasPrevisu");
        BlockLastSwap = serializedObject.FindProperty("BlockLastSwap");

        IsTuto = serializedObject.FindProperty("IsTuto");
        IsTutoRecycling = serializedObject.FindProperty("IsTutoRecycling");
        PlayerForceSwap = serializedObject.FindProperty("PlayerForceSwap");
        HasForcePoseBlocAfterSwap = serializedObject.FindProperty("HasForcePoseBlocAfterSwap");
        ForcePoseBlocCoord = serializedObject.FindProperty("ForcePoseBlocCoord");
        PopUpInfos = serializedObject.FindProperty("PopUpInfos");

        QuestDescription = serializedObject.FindProperty("QuestDescription");
        QuestDescriptionEnglish = serializedObject.FindProperty("QuestDescriptionEnglish");

        QuestFloor = serializedObject.FindProperty("QuestFloor");
        QuestFlower = serializedObject.FindProperty("QuestFlower");
        QuestNoSpecificTiles = serializedObject.FindProperty("QuestNoSpecificTiles");
        QuestTileChain = serializedObject.FindProperty("QuestTileChain");
        NumberTileChain = serializedObject.FindProperty("NumberTileChain");
        QuestTileCount = serializedObject.FindProperty("QuestTileCount");
        NumberTileCount = serializedObject.FindProperty("NumberTileCount");

        DialogLevelStart = serializedObject.FindProperty("DialogLevelStart");
        DialogLevelEnd = serializedObject.FindProperty("DialogLevelEnd");
        CharacterName = serializedObject.FindProperty("CharacterName");
        CharacterSpritesBeginning = serializedObject.FindProperty("CharacterSpritesBeginning");
        CharacterSpritesEnd = serializedObject.FindProperty("CharacterSpritesEnd");
        DialogBeginning = serializedObject.FindProperty("DialogBeginning");
        DialogEnd = serializedObject.FindProperty("DialogEnd");
        DialogBeginningEnglish = serializedObject.FindProperty("DialogBeginningEnglish");
        DialogEndEnglish = serializedObject.FindProperty("DialogEndEnglish");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DisplayLevel();
        DisplayEnergy();
        DisplayGPE();
        DisplayQuestChoose();
        DisplayQuestInfo();
        DisplayDialogs();

        serializedObject.ApplyModifiedProperties();
    }

    private void DisplayLevel()
    {
        level = EditorGUILayout.BeginFoldoutHeaderGroup(level, "-  Level Infos  -");
        if (level)
        {
            EditorGUILayout.PropertyField(LevelName);
            EditorGUILayout.PropertyField(LevelFolder);

            EditorGUILayout.Space(10);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DisplayEnergy()
    {
        energy = EditorGUILayout.BeginFoldoutHeaderGroup(energy, "-  Energy Info  -");
        if (energy)
        {
            EditorGUILayout.PropertyField(EnergyAtStart);

            EditorGUILayout.PropertyField(HasRecycling);

            if (HasRecycling.boolValue)
            {
                EditorGUILayout.PropertyField(HasInfinitRecycling);
                EditorGUILayout.PropertyField(NbOfRecycling);
                EditorGUILayout.Space(10);
            }

            EditorGUILayout.Space(10);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DisplayGPE()
    {
        mechanics = EditorGUILayout.BeginFoldoutHeaderGroup(mechanics, "-  GPE  -");
        if (mechanics)
        {
            EditorGUILayout.PropertyField(HasInventory);

            if (HasInventory.boolValue)
                DisplayChooseStartNbState();


            EditorGUILayout.PropertyField(OpenMemo);
            EditorGUILayout.PropertyField(HasPrevisu);
            EditorGUILayout.PropertyField(BlockLastSwap);

            EditorGUILayout.Space(5);

            EditorGUILayout.PropertyField(IsTutoRecycling);
            EditorGUILayout.PropertyField(IsTuto);

            if (IsTuto.boolValue)
            {
                if (HasInventory.boolValue)
                {
                    EditorGUILayout.PropertyField(HasForcePoseBlocAfterSwap);
                    if (HasForcePoseBlocAfterSwap.boolValue)
                        EditorGUILayout.PropertyField(ForcePoseBlocCoord);
                }
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
        if (mechanics)
        {
            if (IsTuto.boolValue)
            {
                EditorGUILayout.PropertyField(PlayerForceSwap, true);
            }

            EditorGUILayout.Space(7);

            EditorGUILayout.PropertyField(PopUpInfos, true);

            EditorGUILayout.Space(10);
        }
    }

    private void DisplayChooseStartNbState()
    {
        StartNbAllState.arraySize = 10;
        LevelData _levelData = (LevelData)target;

        if (_levelData.StartNbState == AllStates.None)
            _levelData.StartNbState = AllStates.__Grassias__;
        if (_levelData.StartNbState == AllStates.__Pyreneos__)
            _levelData.StartNbState = AllStates.__Viscosa__;

        EditorGUILayout.PropertyField(StartNbState);

        var stateNb = (int)_levelData.StartNbState;
        EditorGUILayout.PropertyField(StartNbAllState.GetArrayElementAtIndex(stateNb));

        EditorGUILayout.Space(10);
    }

    private void DisplayQuestInfo()
    {
        questsInfo = EditorGUILayout.BeginFoldoutHeaderGroup(questsInfo, "-  Quest(s) Infos  -");
        if (questsInfo)
        {
            // EditorGUILayout.PropertyField(QuestImage);
            EditorGUILayout.PropertyField(QuestDescription);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(QuestDescriptionEnglish);

            EditorGUILayout.Space(questsChoose ? 5 : 10);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DisplayQuestChoose()
    {
        if (questsChoose)
            EditorGUILayout.Space(!questsInfo ? 10 : 5);

        questsChoose = EditorGUILayout.BeginFoldoutHeaderGroup(questsChoose, "-  Quest(s) Choose  -");
        EditorGUILayout.EndFoldoutHeaderGroup();
        if (questsChoose)
        {
            EditorGUILayout.PropertyField(QuestFloor);
            EditorGUILayout.PropertyField(QuestFlower);
            EditorGUILayout.PropertyField(QuestNoSpecificTiles);
            EditorGUILayout.PropertyField(QuestTileChain);

            if (QuestTileChain.isExpanded)
                EditorGUILayout.PropertyField(NumberTileChain);
            EditorGUILayout.PropertyField(QuestTileCount);
            if (QuestTileCount.isExpanded)
                EditorGUILayout.PropertyField(NumberTileCount);

            EditorGUILayout.Space(10);
        }
    }

    private void DisplayDialogs()
    {
        dialogs = EditorGUILayout.BeginFoldoutHeaderGroup(dialogs, "-  Dialogs Infos  -");
        if (dialogs)
        {
            EditorGUILayout.PropertyField(DialogLevelStart);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(DialogLevelEnd);
            
            EditorGUILayout.Space(50);
            
            EditorGUILayout.PropertyField(CharacterName);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
        if (dialogs)
        {
            EditorGUILayout.PropertyField(CharacterSpritesBeginning, true);
            EditorGUILayout.PropertyField(CharacterSpritesEnd, true);
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.PropertyField(DialogBeginning);
            EditorGUILayout.PropertyField(DialogEnd);

            EditorGUILayout.Space(10);

            EditorGUILayout.PropertyField(DialogBeginningEnglish);
            EditorGUILayout.PropertyField(DialogEndEnglish);
        }
    }
}