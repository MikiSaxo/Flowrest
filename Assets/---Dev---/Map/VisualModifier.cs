using System;
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
    
    Upgrades CurrentUpgrade{ get; set; }

    [Header("Level 4")] 
    [SerializeField] private Image _level4Img;
    [SerializeField] private Sprite _blindFoldGrassias;
    [SerializeField] private Sprite _blindFoldCalcid;

    [Header("Level 7")] 
    [SerializeField] private Image _level7Img_ClothesPeg;
    [SerializeField] private Image _level7Img_MakeUpArmy;

    [Header("Level 10")] 
    [SerializeField] private GameObject _water;
    [SerializeField] private GameObject _potoKohLanta;
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

    private void Awake()
    {
        Instance = this;
    }

    private void UpdateModification()
    {
        Level4();
        Level7();
        // Level10();
        // Level12();
        // Level14();
        // Level15();
    }

    public void UpdateUpgrades(Upgrades newUpgrade)
    {
        CurrentUpgrade = newUpgrade;
        UpdateModification();
    }

    private void Level4()
    {
        if (CurrentUpgrade == Upgrades.IsBlindFoldGrassias)
        {
            _level4Img.enabled = true;
            _level4Img.sprite = _blindFoldGrassias;
        }
        
        if (CurrentUpgrade == Upgrades.IsBlindFoldCalcid)
        {
            _level4Img.enabled = true;
            _level4Img.sprite = _blindFoldCalcid;
        }
        
        // _level4Img.enabled = false;
        // if (IsBlindFoldGrassias && !IsBlindFoldCalcid)
        // {
        //     _level4Img.enabled = true;
        //     _level4Img.sprite = _blindFoldGrassias;
        // }
        //
        // if (!IsBlindFoldGrassias && IsBlindFoldCalcid)
        // {
        //     _level4Img.enabled = true;
        //     _level4Img.sprite = _blindFoldCalcid;
        // }
    }

    private void Level7()
    {
        if (CurrentUpgrade == Upgrades.IsStinking)
        {
            _level7Img_ClothesPeg.enabled = true;
        }

        if (CurrentUpgrade == Upgrades.IsRambo)
        {
            _level7Img_MakeUpArmy.enabled = true;
        }
        
        // _level7Img.enabled = false;
        // if (IsStinking && !IsRambo)
        // {
        //     _level7Img.enabled = true;
        //     _level7Img.sprite = _clothesPeg;
        // }
        //
        // if (!IsStinking && IsRambo)
        // {
        //     _level7Img.enabled = true;
        //     _level7Img.sprite = _makeUpArmy;
        // }
    }

    private void Level10()
    {
        if (CurrentUpgrade == Upgrades.IsWaterColor)
        {
            // Change Material Water
            var waterMat = _water.GetComponent<MeshRenderer>();
            _potoKohLanta.SetActive(false);
        }

        if (CurrentUpgrade == Upgrades.IsKohLanta)
        {
            // Change Material Water
            var waterMat = _water.GetComponent<MeshRenderer>();
            _potoKohLanta.SetActive(true);
        }
        
        // if (IsWaterColor && !IsKohLanta)
        // {
        //     // Change Material Water
        //     var waterMat = _water.GetComponent<MeshRenderer>();
        //     _potoKohLanta.SetActive(false);
        // }
        //
        // if (!IsWaterColor && IsKohLanta)
        // {
        //     // Change Material Water
        //     var waterMat = _water.GetComponent<MeshRenderer>();
        //     _potoKohLanta.SetActive(true);
        // }
    }

    private void Level12()
    {
        _level12Img.enabled = false;

        if (CurrentUpgrade == Upgrades.IsGoMuscu)
        {
            _level12Img.enabled = true;
            _level12Img.sprite = _goMuscu;
        }

        if (CurrentUpgrade == Upgrades.IsPoor)
        {
            _level12Img.enabled = true;
            _level12Img.sprite = _poorProf;
        }
        
        // if (IsGoMuscu && !IsPoor)
        // {
        //     _level12Img.enabled = true;
        //     _level12Img.sprite = _goMuscu;
        // }
        //
        // if (!IsGoMuscu && IsPoor)
        // {
        //     _level12Img.enabled = true;
        //     _level12Img.sprite = _poorProf;
        // }
    }

    private void Level14()
    {
        if (CurrentUpgrade == Upgrades.IsFootball)
        {
            _level14Img.enabled = true;
            _level14Img.sprite = _nbTen;
        }

        if (CurrentUpgrade == Upgrades.IsGold)
        {
            _level14Img.enabled = false;
            // Change Material Rocks, Water, Cursor and FX Bling Bling 
        }
        
        // _level14Img.enabled = false;
        //
        // if (IsFootball && !IsGold)
        // {
        //     _level14Img.enabled = true;
        //     _level14Img.sprite = _nbTen;
        // }
        //
        // if (!IsFootball && IsGold)
        // {
        //     _level14Img.enabled = false;
        //     // Change Material Rocks, Water, Cursor and FX Bling Bling 
        // }
        
        // if(_currentUpgrade == Upgrades.IsFootball || _currentUpgrade == Upgrades.IsGold)
            // _level14Img.enabled = false;

     
    }

    private void Level15()
    {
        // _level15Img.enabled = false;

        if (CurrentUpgrade == Upgrades.IsIceberg)
        {
            _level15Img.enabled = true;
            _level15Img.sprite = _nbIceProf;
        }

        if (CurrentUpgrade == Upgrades.IsIcePig)
        {
            _level15Img.enabled = false;
            
            if (_stockIceberg == null)
            {
                GameObject go = Instantiate(_icebergs, transform);
                _stockIceberg = go;
            }
        }
        
        // _level15Img.enabled = false;
        //
        // if (IsIceberg && !IsIcePig)
        // {
        //     _level15Img.enabled = true;
        //     _level15Img.sprite = _nbIceProf;
        // }
        //
        // if (!IsIceberg && IsIcePig)
        // {
        //     _level15Img.enabled = false;
        //     
        //     if (_stockIceberg == null)
        //     {
        //         GameObject go = Instantiate(_icebergs, transform);
        //         _stockIceberg = go;
        //     }
        // }
    }
}