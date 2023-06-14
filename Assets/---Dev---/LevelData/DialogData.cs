using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog X", menuName = "Flowrest/Dialog Data")]
public class DialogData : ScriptableObject
{
    [Header("Character Infos")]
    public string CharacterName;
    // public Sprite[] CharacterSprites;

    [Header("Core Dialog")]
    
    public DialogCore[] CoreDialogFrench;
    
    [Space(5f)]
    
    public DialogCore[] CoreDialogEnglish;
    // public string[] CoreDialog;
    // public string[] CoreDialogEnglish;

    [Space(10f)]

    public DialogChoice[] Answers;
}
