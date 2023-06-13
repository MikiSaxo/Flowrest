using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog X", menuName = "Flowrest/Dialog Data")]
public class DialogData : ScriptableObject
{
    public string[] CoreDialog;

    public DialogChoice[] Answers;
}
