using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Dialog X", menuName = "Flowrest/Dialog Data")]
public class DialogData : ScriptableObject
{
    [Header("Character Infos")]
    public string CharacterName;

    [Header("Core Dialog")]
    public DialogCore[] DialogFrench;
    public DialogCore[] DialogEnglish;

    [Space(10f)]

    public DialogChoice[] Choices;
    
    [Space(10f)]
    
    public DialogData NextDialogNoChoice;
    public LevelData LevelToLoad;
}
