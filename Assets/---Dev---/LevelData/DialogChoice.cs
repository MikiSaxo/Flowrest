using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogChoice
{
    public string Choice;
    
    [Space(5f)]
    
    public string ChoiceEnglish;

    [Space(10f)]

    public DialogData NextDialogData;
}
