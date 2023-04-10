using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogPrefab : MonoBehaviour
{
    public TMP_Text DialogText;
    
    [SerializeField] private Vector2 _padding;

    private float _textSizeY;

    public void Init(string dialog)
    {
        DialogText.SetText(dialog);
        DialogText.ForceMeshUpdate();

        Vector2 textSize = DialogText.GetRenderedValues(false);
        _textSizeY = textSize.y;
        DialogText.SetText(String.Empty);
        gameObject.GetComponent<RectTransform>().DOSizeDelta(textSize + _padding, 0);
    }

    public float GetDialogSizeY()
    {
        return _textSizeY;
    }
}
