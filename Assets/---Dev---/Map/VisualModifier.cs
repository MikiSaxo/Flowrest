using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VisualModifier : MonoBehaviour
{
    public static VisualModifier Instance;

    public Upgrades CurrentUpgrade { get; set; }
    public bool IsGold { get; private set; }
    public bool IsColored { get; private set; }

    [Header("Level 4")] [SerializeField] private Image _level4Img;
    [SerializeField] private Sprite _blindFoldGrassias;
    [SerializeField] private Sprite _blindFoldCalcid;

    [Header("Level 7")] [SerializeField] private Image _level7Img_ClothesPeg;
    [SerializeField] private GameObject _level7_StinkingMussel;
    [SerializeField] private Image _level7Img_Rambo;

    [Header("Level 10")] [SerializeField] private GameObject _water;
    [SerializeField] private Color _waterBase;
    [SerializeField] private GameObject _seaBottle;
    [SerializeField] private Material _waterColored;
    [SerializeField] private Color _waterTubeColored;

    [Header("Level 13")] [SerializeField] private Image _level13Img_GoMuscu;
    [SerializeField] private Image _level13Img_MakeUp;
    [SerializeField] private Image _level13Img_PoorProf;

    [Header("Level 15")] [SerializeField] private Image _level15Img_Supporter;
    [SerializeField] private GameObject[] _goldRocks;
    [SerializeField] private GameObject _fXBlingBling;
    [SerializeField] private Material _goldMat;
    [SerializeField] private Material _waterGold;
    [SerializeField] private Color _waterTubeGold;

    [Header("Level 16")] [SerializeField] private Image _level16Img_MouthIce;
    [SerializeField] private GameObject _icebergs;

    private GameObject _stockIceberg;
    private bool _hasUnlockedLvl4;
    private bool _hasUnlockedLvl7;
    private bool _hasUnlockedLvl10;
    private bool _hasUnlockedLvl13;
    private bool _hasUnlockedLvl13GoMuscu;
    private bool _hasUnlockedLvl15;
    private bool _hasUnlockedLvl16;
    private bool _hasUnlockedLvl18;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // _hasUnlockedLvl4 = true;
        // _hasUnlockedLvl7 = true;
        // _hasUnlockedLvl13 = true;
        // _hasUnlockedLvl15 = true;
        // _hasUnlockedLvl16 = true;
        // _hasUnlockedLvl18 = true;
    }

    public void UpdateCharacters(Characters whichChara)
    {
        if (whichChara == Characters.Profess)
        {
            if (_hasUnlockedLvl4)
                _level4Img.enabled = true;
            if (_hasUnlockedLvl7)
            {
                _level7Img_ClothesPeg.enabled = true;
                _level7Img_Rambo.enabled = false;
            }

            if (_hasUnlockedLvl13)
            {
                if (_hasUnlockedLvl13GoMuscu)
                {
                    _level13Img_GoMuscu.enabled = true;
                    _level13Img_MakeUp.enabled = true;
                    _level13Img_PoorProf.enabled = false;
                }
                else
                {
                    _level13Img_GoMuscu.enabled = false;
                    _level13Img_MakeUp.enabled = false;
                    _level13Img_PoorProf.enabled = true;
                }
            }

            if (_hasUnlockedLvl15)
                _level15Img_Supporter.enabled = true;
            if (_hasUnlockedLvl16)
                _level16Img_MouthIce.enabled = false;
        }
        else if (whichChara == Characters.DG)
        {
            if (_hasUnlockedLvl4)
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
                _level13Img_PoorProf.enabled = false;
            }

            if (_hasUnlockedLvl15)
                _level15Img_Supporter.enabled = false;
            if (_hasUnlockedLvl16)
                _level16Img_MouthIce.enabled = true;
        }
        else
        {
            _level4Img.enabled = false;
            _level7Img_ClothesPeg.enabled = false;
            _level7Img_Rambo.enabled = false;
            _level13Img_GoMuscu.enabled = false;
            _level13Img_MakeUp.enabled = false;
            _level13Img_PoorProf.enabled = false;
            _level15Img_Supporter.enabled = false;
            _level16Img_MouthIce.enabled = false;
        }
    }

    private void UpdateModification()
    {
        Level4();
        Level7();
        Level10();
        Level13();
        Level15();
        Level16();
        Level18();
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
            _seaBottle.SetActive(false);
            _hasUnlockedLvl10 = true;
            var getWaterMesh = _water.GetComponent<MeshRenderer>();
            getWaterMesh.material = _waterColored;
            IsColored = true;
        }

        if (CurrentUpgrade == Upgrades.IsKohLanta)
        {
            _seaBottle.SetActive(true);
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
            _hasUnlockedLvl13GoMuscu = true;
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
            IsGold = true;

            // Change Material Rocks, Water, Cursor and FX Bling Bling 
            _fXBlingBling.SetActive(true);

            foreach (var rock in _goldRocks)
            {
                var getMesh = rock.GetComponent<MeshRenderer>();
                getMesh.material = _goldMat;
            }
            
            var getWaterMesh = _water.GetComponent<MeshRenderer>();
            getWaterMesh.material = _waterGold;
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

    private void Level18()
    {
        
    }

    public Color GetWaterTubeBase()
    {
        return _waterBase;
    }
    public Color GetWaterTubeColored()
    {
        return _waterTubeColored;
    }
    public Color GetWaterTubeGold()
    {
        return _waterTubeGold;
    }
}