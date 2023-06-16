using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class DialogChoice
{
    public string Choice;
    public string ChoiceEnglish;

    [Space(10f)]

    public DialogData NextDialogData;

    [FormerlySerializedAs("LevelNoDiaData")] [FormerlySerializedAs("LevelNoDialog")] [FormerlySerializedAs("NextLevelNoDialog")] [Space(5f)]
    
    public LevelData LevelToLoad;
    
    [Space(10f)]
    
    public DialogData EndDialog;
}
