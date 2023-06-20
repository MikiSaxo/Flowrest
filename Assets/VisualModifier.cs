using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VisualModifier : MonoBehaviour
{
    public static VisualModifier Instance;

    public bool IsBlindFoldGrassias { get; set; }
    public bool IsBlindFoldCalcid { get; set; }
    public bool IsStinking { get; set; }
    public bool IsRambo { get; set; }
    public bool IsWaterColor { get; set; }
    public bool IsKohLanta { get; set; }
    public bool IsGoMuscu { get; set; }
    public bool IsPoor { get; set; }
    public bool IsFootball { get; set; }
    public bool IsGold { get; set; }
    public bool IsIceberg { get; set; }
    public bool IsIcePig { get; set; }

    [Header("Level 4")] 
    [SerializeField] private Image _level4Img;
    [SerializeField] private Sprite _blindFoldGrassias;
    [SerializeField] private Sprite _blindFoldCalcid;

    [Header("Level 7")] 
    [SerializeField] private Image _level7Img;
    [SerializeField] private Sprite _clothesPeg;
    [SerializeField] private Sprite _makeUpArmy;

    [Header("Level 10")] 
    [SerializeField] private GameObject _water;
    [SerializeField] private GameObject _postKohLanta;
    [SerializeField] private Material _waterColored;

    [Header("Level 12")] 
    [SerializeField] private Image _level12Img;
    [SerializeField] private Sprite _goMuscu;
    [SerializeField] private Sprite _poorProf;

    [Header("Level 14")] 
    [SerializeField] private Image _level14Img;
    [SerializeField] private Sprite _nbTen;
    [SerializeField] private GameObject[] _goldRocks;
    [SerializeField] private GameObject _fXBlingBling;
    [SerializeField] private Material _goldMat;

    [Header("Level 15")] 
    [SerializeField] private Image _level15Img;
    [SerializeField] private Sprite _nbIceProf;
    [SerializeField] private GameObject _icebergs;

    private GameObject _stockIceberg;

    public void UpdateModification()
    {
        Level4();
        Level7();
        Level10();
        Level12();
        Level14();
        Level15();
    }

    private void Level4()
    {
        _level4Img.enabled = false;

        if (IsBlindFoldGrassias && !IsBlindFoldCalcid)
        {
            _level4Img.enabled = true;
            _level4Img.sprite = _blindFoldGrassias;
        }

        if (!IsBlindFoldGrassias && IsBlindFoldCalcid)
        {
            _level4Img.enabled = true;
            _level4Img.sprite = _blindFoldCalcid;
        }
    }

    private void Level7()
    {
        _level7Img.enabled = false;

        if (IsStinking && !IsRambo)
        {
            _level7Img.enabled = true;
            _level7Img.sprite = _clothesPeg;
        }

        if (!IsStinking && IsRambo)
        {
            _level7Img.enabled = true;
            _level7Img.sprite = _makeUpArmy;
        }
    }

    private void Level10()
    {
        if (IsWaterColor && !IsKohLanta)
        {
            // Change Material Water
            var waterMat = _water.GetComponent<MeshRenderer>();
            _postKohLanta.SetActive(false);
        }

        if (!IsWaterColor && IsKohLanta)
        {
            // Change Material Water
            var waterMat = _water.GetComponent<MeshRenderer>();
            _postKohLanta.SetActive(true);
        }
    }

    private void Level12()
    {
        _level12Img.enabled = false;

        if (IsGoMuscu && !IsPoor)
        {
            _level12Img.enabled = true;
            _level12Img.sprite = _goMuscu;
        }

        if (!IsGoMuscu && IsPoor)
        {
            _level12Img.enabled = true;
            _level12Img.sprite = _poorProf;
        }
    }

    private void Level14()
    {
        _level14Img.enabled = false;

        if (IsFootball && !IsGold)
        {
            _level14Img.enabled = true;
            _level14Img.sprite = _nbTen;
        }

        if (!IsFootball && IsGold)
        {
            _level14Img.enabled = false;
            // Change Material Rocks, Water, Cursor and FX Bling Bling 
        }
    }

    private void Level15()
    {
        _level15Img.enabled = false;

        if (IsIceberg && !IsIcePig)
        {
            _level15Img.enabled = true;
            _level15Img.sprite = _nbIceProf;
        }

        if (!IsIceberg && IsIcePig)
        {
            _level15Img.enabled = false;
            
            if (_stockIceberg == null)
            {
                GameObject go = Instantiate(_icebergs, transform);
                _stockIceberg = go;
            }
        }
    }
}