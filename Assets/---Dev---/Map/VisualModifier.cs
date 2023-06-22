using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VisualModifier : MonoBehaviour
{
    public static VisualModifier Instance;

    Upgrades CurrentUpgrade{ get; set; }
    Characters CurrentCharacter{ get; set; }

    [Header("Level 4")] 
    [SerializeField] private Image _level4Img;
    [SerializeField] private Sprite _blindFoldGrassias;
    [SerializeField] private Sprite _blindFoldCalcid;

    [Header("Level 7")] 
    [SerializeField] private Image _level7Img_ClothesPeg;
    [SerializeField] private GameObject _level7_StinkingMussel;
    [SerializeField] private Image _level7Img_Rambo;

    [Header("Level 10")] 
    [SerializeField] private GameObject _water;
    [SerializeField] private GameObject _potoKohLanta;
    [SerializeField] private Material _waterColored;

    [Header("Level 13")] 
    [SerializeField] private Image _level13Img_GoMuscu;
    [SerializeField] private Image _level13Img_MakeUp;
    [SerializeField] private Image _level13Img_PoorProf;

    [Header("Level 15")] 
    [SerializeField] private Image _level15Img_Supporter;
    [SerializeField] private GameObject[] _goldRocks;
    [SerializeField] private GameObject _fXBlingBling;
    [SerializeField] private Material _goldMat;

    [Header("Level 16")] 
    [SerializeField] private Image _level16Img_MouthIce;
    [SerializeField] private GameObject _icebergs;

    private GameObject _stockIceberg;
    private bool _hasUnlockedLvl4;
    private bool _hasUnlockedLvl7;
    private bool _hasUnlockedLvl10;
    private bool _hasUnlockedLvl13;
    private bool _hasUnlockedLvl15;
    private bool _hasUnlockedLvl16;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateCharacters(Characters whichChara)
    {
        if (whichChara == Characters.Profess)
        {
            if(_hasUnlockedLvl4)
                _level4Img.enabled = true;
            if (_hasUnlockedLvl7)
            {
                _level7Img_ClothesPeg.enabled = true;
                _level7Img_Rambo.enabled = false;
            }
            if (_hasUnlockedLvl13)
            {
                _level13Img_GoMuscu.enabled = true;
                _level13Img_MakeUp.enabled = true;
            }
            if (_hasUnlockedLvl15)
                _level15Img_Supporter.enabled = true;
            if (_hasUnlockedLvl16)
                _level16Img_MouthIce.enabled = false;
        }
        if (whichChara == Characters.DG)
        {
            if(_hasUnlockedLvl4)
                _level4Img.enabled = false;
            if (_hasUnlockedLvl7)
            {
                _level7Img_ClothesPeg.enabled = false;
                _level7Img_Rambo.enabled = true;
            }
            if (_hasUnlockedLvl13)
            {
                _level13Img_GoMuscu.enabled = false;
                _level13Img_MakeUp.enabled = false;
            }
            if (_hasUnlockedLvl15)
                _level15Img_Supporter.enabled = false;
            if (_hasUnlockedLvl16)
                _level16Img_MouthIce.enabled = true;
        }
    }
    private void UpdateModification()
    {
        Level4();
        Level7();
        // Level10();
        Level13();
        Level15();
        Level16();
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
            _hasUnlockedLvl4 = true;
        }
        
        if (CurrentUpgrade == Upgrades.IsBlindFoldCalcid)
        {
            _level4Img.enabled = true;
            _level4Img.sprite = _blindFoldCalcid;
            _hasUnlockedLvl4 = true;
        }
    }

    private void Level7()
    {
        if (CurrentUpgrade == Upgrades.IsStinking)
        {
            _level7Img_ClothesPeg.enabled = true;
            // _level7Img_Rambo.enabled = false;
            _level7_StinkingMussel.SetActive(true);
            _hasUnlockedLvl7 = true;
        }

        if (CurrentUpgrade == Upgrades.IsRambo)
        {
            // _level7Img_ClothesPeg.enabled = false;
            _level7Img_Rambo.enabled = true;
            // _level7_StinkingMussel.SetActive(false);
            _hasUnlockedLvl7 = true;
        }
    }

    private void Level10()
    {
        if (CurrentUpgrade == Upgrades.IsWaterColor)
        {
            // Change Material Water
            var waterMat = _water.GetComponent<MeshRenderer>();
            _potoKohLanta.SetActive(false);
            _hasUnlockedLvl10 = true;
        }

        if (CurrentUpgrade == Upgrades.IsKohLanta)
        {
            // Change Material Water
            var waterMat = _water.GetComponent<MeshRenderer>();
            _potoKohLanta.SetActive(true);
            _hasUnlockedLvl10 = true;
        }
    }

    private void Level13()
    {
        if (CurrentUpgrade == Upgrades.IsGoMuscu)
        {
            _level13Img_GoMuscu.enabled = true;
            _level13Img_MakeUp.enabled = true;
            _level13Img_PoorProf.enabled = false;
            _hasUnlockedLvl13 = true;
        }

        if (CurrentUpgrade == Upgrades.IsPoor)
        {
            _level13Img_GoMuscu.enabled = false;
            _level13Img_MakeUp.enabled = false;
            _level13Img_PoorProf.enabled = true;
            _hasUnlockedLvl13 = true;
        }
    }

    private void Level15()
    {
        if (CurrentUpgrade == Upgrades.IsFootball)
        {
            _level15Img_Supporter.enabled = true;
            _hasUnlockedLvl15 = true;
        }

        if (CurrentUpgrade == Upgrades.IsGold)
        {
            _hasUnlockedLvl15 = true;
            // Change Material Rocks, Water, Cursor and FX Bling Bling 
        }
    }

    private void Level16()
    {
        if (CurrentUpgrade == Upgrades.IsIceberg)
        {
            _level16Img_MouthIce.enabled = true;
            _icebergs.SetActive(false);
            _hasUnlockedLvl16 = true;
        }

        if (CurrentUpgrade == Upgrades.IsIcePig)
        {
            _level16Img_MouthIce.enabled = false;
            _icebergs.SetActive(true);
            _hasUnlockedLvl16 = true;
        }
    }
}