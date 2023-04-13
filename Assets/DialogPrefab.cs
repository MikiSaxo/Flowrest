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

    public void Init(string dialog, float dialogSpeed)
    {
        IsFinish = false;
        DialogText.SetText(dialog);
        DialogText.ForceMeshUpdate();

        _saveDialog = dialog;
        _dialogSpeed = dialogSpeed;

        DialogText.SetText(String.Empty);
        //Vector2 textSize = DialogText.GetRenderedValues(false);
        //_textSizeY = textSize.y;
        //gameObject.GetComponent<RectTransform>().DOSizeDelta(textSize + _padding, 0);

        StartCoroutine(AnimationText());
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

        // ScreensManager.Instance.CheckIfDialogEnded();
        // ScreensManager.Instance.SpawnAllDialog();
    }

    public float GetDialogSizeY()
    {
        return _textSizeY;
    }
}