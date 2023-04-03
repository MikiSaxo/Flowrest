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
            
            if(QuestTileChain.isExpanded)
                EditorGUILayout.PropertyField(NumberTileChain);
            EditorGUILayout.PropertyField(QuestTileCount);
            if(QuestTileCount.isExpanded)
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