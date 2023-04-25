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

    public bool IsFinish { get; set; }

    [SerializeField] private Vector2 _padding;

    private float _textSizeY;
    private float _dialogSpeed;
    private bool _stopCorou;
    private string _saveDialog;
    
    private int _maxNb;
    private int _currentNb;
    private string _currentOrder;

    public void Init(string dialog, float dialogSpeed)
    {
        IsFinish = false;
        // dialog = $"{dialog}\nblou";
        if (dialog.Contains('$'))
        {
            var replace = dialog.Replace('$', '\n');
            dialog = replace;
        }

        DialogText.text = dialog;
        DialogText.ForceMeshUpdate();

        _saveDialog = dialog;
        _dialogSpeed = dialogSpeed;

        DialogText.SetText(String.Empty);
        //Vector2 textSize = DialogText.GetRenderedValues(false);
        //_textSizeY = textSize.y;
        //gameObject.GetComponent<RectTransform>().DOSizeDelta(textSize + _padding, 0);

        StartCoroutine(AnimationText());
    }


    public void InitDescOrder(string text)
    {
        DialogText.text = text;
        DialogText.ForceMeshUpdate();
      
        Vector2 textSize = DialogText.GetRenderedValues(false);
        _textSizeY = textSize.y;
        gameObject.GetComponent<RectTransform>().DOSizeDelta(textSize + _padding, 0);
    }
    public void InitOrder(string desc, int nbToReach)
    {
        _currentOrder = desc;
        _maxNb = nbToReach;
        
        UpdateText();
        DialogText.ForceMeshUpdate();
      
        Vector2 textSize = DialogText.GetRenderedValues(false);
        _textSizeY = textSize.y;
        gameObject.GetComponent<RectTransform>().DOSizeDelta(textSize + _padding, 0);
    }

    public void UpdateCurrentNbOrder(int nb)
    {
        _currentNb = nb;
        UpdateText();
    }

    private void UpdateText()
    {
        DialogText.text = $"{_currentOrder} : {_currentNb} / {_maxNb}";
    }

    public void UpdateMaxNb(int nb)
    {
        _maxNb = nb;
    }

    IEnumerator AnimationText()
    {
        int charIndex = 0;
        
        foreach (char c in _saveDialog)
        {
            if (_stopCorou)
            {
                _stopCorou = false;
                yield break;
            }

            charIndex++;

            // Display the first part of the text
            var firstText = _saveDialog.Substring(0, charIndex);
            
            // Make the part non spawned non-visible
            var secondText = $"<color=#00000000>{_saveDialog.Substring(charIndex)}";
            
            // Update text with the two part
            DialogText.text = firstText + secondText;

            ScreensManager.Instance.GoToBottomScrollBar();
            yield return new WaitForSeconds(_dialogSpeed);
        }

        EndAnimationText();
    }

    public void EndAnimationText()
    {
        _stopCorou = true;
        DialogText.text = _saveDialog;
        StopCoroutine(AnimationText());
        IsFinish = true;

        ScreensManager.Instance.UpdateDialogFB(true);
        // ScreensManager.Instance.CheckIfDialogEnded();
        // ScreensManager.Instance.SpawnAllDialog();
    }

    public float GetDialogSizeY()
    {
        return _textSizeY;
    }
}