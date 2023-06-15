using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog X", menuName = "Flowrest/Dialog Data")]
public class DialogData : ScriptableObject
{
    [Header("Character Infos")]
    public string CharacterName;

    [Header("Core Dialog")]
    
    public DialogCore[] CoreDialogFrench;
    
    [Space(5f)]
    
    public DialogCore[] CoreDialogEnglish;

    [Space(10f)]

    public DialogChoice[] Choices;
}
