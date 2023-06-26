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

    public DialogData AddDialog;

    [Space(5f)]
    
    public LevelData LevelToLoad;
    
    [Space(10f)]
    
    public DialogData NextLevelDialog;
    
    [Space(10f)]
    
    public DialogData DialogOfEnd;
    
    [Space(10f)]
    
    public DialogUpgrades VisualUpgrades;
}
