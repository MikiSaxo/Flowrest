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
    
    [Space(10f)]
    
    [Header("-- If no choice Next Dialog --")]
    public DialogData NextDialog;
    
    [Space(10f)]
    
    [Header("-- If no choice Level To Load --")]
    public LevelData LevelToLoad;
    
    [Space(10f)]
    
    [Header("-- If no choice End Dialog --")]
    public DialogData EndDialog;
    
    [Space(20f)]
    
    [Header("---- Upgrades ----")]
    public DialogUpgrades VisualUpgrades;
}
