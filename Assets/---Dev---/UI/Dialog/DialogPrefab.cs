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

    [SerializeField] private TextReplace[] _textModifier;
    [SerializeField] private Vector2 _padding;

    private float _textSizeY;
    private float _dialogSpeed;
    private bool _stopCorou;
    private string _saveDialog;

    private int _maxNb;
    private int _currentNb;
    private string _currentOrder;

    private const float _widthOrder = 333;

    public void Init(string dialog, float dialogSpeed)
    {
        IsFinish = false;
        // dialog = $"{dialog}\nblou";
        if (dialog.Contains('$'))
        {
            var replace = dialog.Replace('$', '\n');
            dialog = replace;
        }

        dialog = UpdateModifierText(dialog);


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

    private string UpdateModifierText(string dialog)
    {
        string replace = dialog;

        foreach (var text in _textModifier)
        {
            if (dialog.Contains(text.Text))
            {
                var color = $"<color=#{ColorUtility.ToHtmlStringRGBA(text.Color)}>";

                if (text.IsColor && text.IsBold && text.IsItalic)
                {
                    replace = dialog.Replace(text.Text, $"<b><i>{color}{text.Text}</color></i></b>");
                }
                else if (text.IsColor && text.IsBold)
                {
                    replace = dialog.Replace(text.Text, $"<b>{color}{text.Text}</color></b>");
                }
                else if (text.IsColor && text.IsItalic)
                {
                    replace = dialog.Replace(text.Text, $"<i>{color}{text.Text}</color></i>");
                }
                else if (text.IsBold && text.IsItalic)
                {
                    replace = dialog.Replace(text.Text, $"<b><i>{text.Text}</i></b>");
                }
                else if (!text.IsColor && text.IsBold)
                {
                    replace = dialog.Replace(text.Text, $"<b>{text.Text}<b>");
                }
                else if (!text.IsColor && text.IsItalic)
                {
                    replace = dialog.Replace(text.Text, $"<i>{text.Text}</i>");
                }
                else
                    replace = dialog.Replace(text.Text, $"{color}{text.Text}</color>");

                dialog = replace;
            }
        }

        return dialog;
    }

    public void InitDescOrder(string text)
    {
        text = UpdateModifierText(text);

        DialogText.text = text;
        DialogText.ForceMeshUpdate();

        Vector2 textSize = DialogText.GetRenderedValues(false);
        _textSizeY = textSize.y;
        gameObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(_widthOrder, _textSizeY - 20), 0);
    }

    public void InitOrder(string desc, int nbToReach)
    {
        desc = UpdateModifierText(desc);

        _currentOrder = desc;
        _maxNb = nbToReach;

        DialogText.text = desc;
        DialogText.ForceMeshUpdate();

        Vector2 textSize = DialogText.GetRenderedValues(false);
        _textSizeY = textSize.y;
        gameObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(_widthOrder, _textSizeY + _padding.y), 0);

        UpdateText();
    }

    public void UpdateCurrentNbOrder(int nb)
    {
        _currentNb = nb;
        UpdateText();
    }

    public void AddNewNbOrder(int nb)
    {
        _currentNb += nb;
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


            var checkSecond = String.Empty;

            // if (charIndex + 7 < _saveDialog.Length - 1)
            // {
            //     checkSecond = _saveDialog.Substring(charIndex, 7);
            //
            //     if (checkSecond == "<color=")
            //         charIndex += 17;
            //
            //     if (checkSecond == "</color")
            //         charIndex += 8;
            //     
            //     
            //  
            //     
            //    
            // }

            if (charIndex + 3 < _saveDialog.Length - 1)
            {
                checkSecond = _saveDialog.Substring(charIndex, 3);
                if (checkSecond == "<b>")
                    charIndex += 3;
                
                checkSecond = _saveDialog.Substring(charIndex, 3);
                if (checkSecond == "<i>")
                    charIndex += 3;
               
                checkSecond = _saveDialog.Substring(charIndex, 3);
                if (checkSecond == "<co")
                    charIndex += 17;
                
                
                
                checkSecond = _saveDialog.Substring(charIndex, 3);
                if (checkSecond == "</c")
                    charIndex += 8;
                
                checkSecond = _saveDialog.Substring(charIndex, 3);
                if (checkSecond == "</i")
                    charIndex += 4;

                checkSecond = _saveDialog.Substring(charIndex, 3);
                if (checkSecond == "</b")
                    charIndex += 4;
            }


            charIndex++;

            charIndex = Mathf.Clamp(charIndex, charIndex, _saveDialog.Length);

            // print("charIndex : " + charIndex + " / " + _saveDialog.Length);

            // Display the first part of the text
            var firstText = _saveDialog.Substring(0, charIndex);

            // Make the part non spawned non-visible
            var secondText = $"{_saveDialog.Substring(charIndex)}";

            if (secondText.Contains("<color=#"))
                secondText = secondText.Replace("<color=#", "");

            if (secondText.Contains("</color>"))
                secondText = secondText.Replace("</color>", "");

            if (secondText.Contains("</b>"))
                secondText = secondText.Replace("</b>", "");
            if (secondText.Contains("<b>"))
                secondText = secondText.Replace("<b>", "");
            if (secondText.Contains("</i>"))
                secondText = secondText.Replace("</i>", "");

            secondText = $"<color=#00000000>{secondText}";

            // Update text with the two part
            DialogText.text = firstText + secondText;

            // ScreensManager.Instance.GoToBottomScrollBar();
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