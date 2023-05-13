using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _textNumber;

    private int _levelNumber;
    
    public void Init(int levelNumber)
    {
        _textNumber.text = $"{levelNumber}";
        _levelNumber = levelNumber;
    }

    public void OnClick()
    {
        if (BigManager.Instance == null)
        {
            print("Didn't found ----BigManager----");
            return;
        }

        BigManager.Instance.CurrentLevel = _levelNumber-1;
        MainMenuManager.Instance.LaunchMainScene();
    }
}
