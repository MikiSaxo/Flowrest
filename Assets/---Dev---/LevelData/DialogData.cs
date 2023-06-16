using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Dialog X", menuName = "Flowrest/Dialog Data")]
public class DialogData : ScriptableObject
{
    [Header("Core Dialog")]
    public DialogCore[] DialogFrench;
    public DialogCore[] DialogEnglish;

    [Space(10f)]

    [Header("Choices")]
    public DialogChoice[] Choices;
    
    [FormerlySerializedAs("NextDialogNoChoice")]
    [Space(10f)]
    
    [Header("-- If no Next Level --")]
    public LevelData LevelToLoad;
    
    [Space(10f)]
    
    [Header("-- If no End Dialog --")]
    public DialogData NextDialogEndLvl;
}
