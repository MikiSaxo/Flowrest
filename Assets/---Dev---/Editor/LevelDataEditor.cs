using System;
using UnityEditor;

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
    private SerializedProperty HasRecycling;
    private SerializedProperty HasPrevisu;
    private SerializedProperty BlockLastSwap;
    private SerializedProperty PlayerForceChangeThese2Tiles;

    private SerializedProperty QuestDescription;
    private SerializedProperty QuestImage;

    private SerializedProperty QuestFloor;
    private SerializedProperty QuestFlower;
    private SerializedProperty QuestNoSpecificTiles;
    private SerializedProperty QuestTileChain;
    private SerializedProperty NumberTileChain;
    private SerializedProperty QuestTileCount;
    private SerializedProperty NumberTileCount;

    private SerializedProperty CharacterName;
    private SerializedProperty DialogBeginning;
    private SerializedProperty DialogEnd;

    private SerializedProperty StartNbState;
    private SerializedProperty StartNbPlain;
    private SerializedProperty StartNbDesert;
    private SerializedProperty StartNbWater;
    private SerializedProperty StartNbTropical;
    private SerializedProperty StartNbSavanna;
    private SerializedProperty StartNbGeyser;
    private SerializedProperty StartNbSnow;
    private SerializedProperty StartNbPolarDesert;
    private SerializedProperty StartNbTundra;
    private SerializedProperty StartNbSwamp;

    void OnEnable()
    {
        LevelName = serializedObject.FindProperty("LevelName");
        LevelFolder = serializedObject.FindProperty("LevelFolder");

        EnergyAtStart = serializedObject.FindProperty("EnergyAtStart");

        HasInventory = serializedObject.FindProperty("HasInventory");
        HasRecycling = serializedObject.FindProperty("HasRecycling");
        HasPrevisu = serializedObject.FindProperty("HasPrevisu");
        BlockLastSwap = serializedObject.FindProperty("BlockLastSwap");
        PlayerForceChangeThese2Tiles = serializedObject.FindProperty("PlayerForceChangeThese2Tiles");

        QuestDescription = serializedObject.FindProperty("QuestDescription");
        QuestImage = serializedObject.FindProperty("QuestImage");

        QuestFloor = serializedObject.FindProperty("QuestFloor");
        QuestFlower = serializedObject.FindProperty("QuestFlower");
        QuestNoSpecificTiles = serializedObject.FindProperty("QuestNoSpecificTiles");
        QuestTileChain = serializedObject.FindProperty("QuestTileChain");
        NumberTileChain = serializedObject.FindProperty("NumberTileChain");
        QuestTileCount = serializedObject.FindProperty("QuestTileCount");
        NumberTileCount = serializedObject.FindProperty("NumberTileCount");

        CharacterName = serializedObject.FindProperty("CharacterName");
        DialogBeginning = serializedObject.FindProperty("DialogBeginning");
        DialogEnd = serializedObject.FindProperty("DialogEnd");

        StartNbState = serializedObject.FindProperty("StartNbState");
        StartNbPlain = serializedObject.FindProperty("StartNbPlain");
        StartNbDesert = serializedObject.FindProperty("StartNbDesert");
        StartNbWater = serializedObject.FindProperty("StartNbWater");
        StartNbTropical = serializedObject.FindProperty("StartNbTropical");
        StartNbSavanna = serializedObject.FindProperty("StartNbSavanna");
        StartNbGeyser = serializedObject.FindProperty("StartNbGeyser");
        StartNbSnow = serializedObject.FindProperty("StartNbSnow");
        StartNbPolarDesert = serializedObject.FindProperty("StartNbPolarDesert");
        StartNbTundra = serializedObject.FindProperty("StartNbTundra");
        StartNbSwamp = serializedObject.FindProperty("StartNbSwamp");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DisplayLevel();
        DisplayEnergy();
        DisplayMechanics();
        DisplayQuestInfo();
        DisplayQuestChoose();
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

            EditorGUILayout.Space(10);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DisplayMechanics()
    {
        mechanics = EditorGUILayout.BeginFoldoutHeaderGroup(mechanics, "-  GPE  -");
        if (mechanics)
        {
            EditorGUILayout.PropertyField(HasInventory);
            if (HasInventory.boolValue)
                DisplayChooseStartNbState();
            EditorGUILayout.PropertyField(HasRecycling);
            EditorGUILayout.PropertyField(HasPrevisu);
            EditorGUILayout.PropertyField(BlockLastSwap);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
        if (mechanics)
        {
            EditorGUILayout.PropertyField(PlayerForceChangeThese2Tiles, true);

            EditorGUILayout.Space(10);
        }
    }

    private void DisplayChooseStartNbState()
    {
        LevelData _levelData = (LevelData)target;
        
        EditorGUILayout.PropertyField(StartNbState);

        switch (_levelData.StartNbState)
        {
            case AllStates.Plain:
                EditorGUILayout.PropertyField(StartNbPlain);
                break;
            case AllStates.Desert:
                EditorGUILayout.PropertyField(StartNbDesert);
                break;
            case AllStates.Water:
                EditorGUILayout.PropertyField(StartNbWater);
                break;
            case AllStates.Tropical:
                EditorGUILayout.PropertyField(StartNbTropical);
                break;
            case AllStates.Savanna:
                EditorGUILayout.PropertyField(StartNbSavanna);
                break;
            case AllStates.Geyser:
                EditorGUILayout.PropertyField(StartNbGeyser);
                break;
            case AllStates.Snow:
                EditorGUILayout.PropertyField(StartNbSnow);
                break;
            case AllStates.PolarDesert:
                EditorGUILayout.PropertyField(StartNbPolarDesert);
                break;
            case AllStates.Tundra:
                EditorGUILayout.PropertyField(StartNbTundra);
                break;
            case AllStates.Swamp:
                EditorGUILayout.PropertyField(StartNbSwamp);
                break;
        }
    }

    private void DisplayQuestInfo()
    {
        questsInfo = EditorGUILayout.BeginFoldoutHeaderGroup(questsInfo, "-  Quest(s) Infos  -");
        if (questsInfo)
        {
            EditorGUILayout.PropertyField(QuestDescription);
            EditorGUILayout.PropertyField(QuestImage);

            EditorGUILayout.Space(questsChoose ? 5 : 10);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DisplayQuestChoose()
    {
        if (questsChoose)
            EditorGUILayout.Space(!questsInfo ? 10 : 5);

        questsChoose = EditorGUILayout.BeginFoldoutHeaderGroup(questsChoose, "-  Choose Quest(s)  -");
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
            EditorGUILayout.PropertyField(CharacterName);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
        if (dialogs)
        {
            EditorGUILayout.PropertyField(DialogBeginning);
            EditorGUILayout.PropertyField(DialogEnd);
        }
    }
}