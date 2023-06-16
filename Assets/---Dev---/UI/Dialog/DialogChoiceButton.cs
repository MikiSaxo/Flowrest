using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogChoiceButton : MonoBehaviour
{
    private int _choiceIndex;
    
    public void InitChoiceIndex(int index)
    {
        _choiceIndex = index;
    }

    public void OnClick()
    {
        DialogManager.Instance.MakeAChoice(_choiceIndex);
    }
}
