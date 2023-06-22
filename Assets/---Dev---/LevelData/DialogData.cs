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
    
    [Space(5f)]
    
    [Header("-- Dialog of End --")]
    public DialogData DialogOfEnd;
    
    [FormerlySerializedAs("NextDialog")]
    [Space(20f)]
    
    [Header("-- If no choice Next Dialog --")]
    public DialogData AddDialog;
    
    [Space(10f)]
    
    [Header("-- If no choice Level To Load --")]
    public LevelData LevelToLoad;
    
    [FormerlySerializedAs("EndDialog")]
    [Space(10f)]
    
    [Header("-- If no choice Next Level Dialog --")]
    public DialogData NextLevelDialog;

    [Space(20f)]
    
    [Header("---- Upgrades ----")]
    public DialogUpgrades VisualUpgrades;
}
