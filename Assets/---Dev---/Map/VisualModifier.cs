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
    [SerializeField] private Image _level15Img_MouthIce;
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
        }
        
        if (CurrentUpgrade == Upgrades.IsBlindFoldCalcid)
        {
            _level4Img.enabled = true;
            _level4Img.sprite = _blindFoldCalcid;
        }
    }

    private void Level7()
    {
        if (CurrentUpgrade == Upgrades.IsStinking)
        {
            _level7Img_ClothesPeg.enabled = true;
            _level7Img_Rambo.enabled = false;
            _level7_StinkingMussel.SetActive(true);
        }

        if (CurrentUpgrade == Upgrades.IsRambo)
        {
            _level7Img_ClothesPeg.enabled = false;
            _level7Img_Rambo.enabled = true;
            _level7_StinkingMussel.SetActive(false);
        }
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
    }

    private void Level13()
    {
        if (CurrentUpgrade == Upgrades.IsGoMuscu)
        {
            _level13Img_GoMuscu.enabled = true;
            _level13Img_MakeUp.enabled = true;
            _level13Img_PoorProf.enabled = false;
        }

        if (CurrentUpgrade == Upgrades.IsPoor)
        {
            _level13Img_GoMuscu.enabled = false;
            _level13Img_MakeUp.enabled = false;
            _level13Img_PoorProf.enabled = true;
        }
    }

    private void Level15()
    {
        if (CurrentUpgrade == Upgrades.IsFootball)
        {
            _level15Img_Supporter.enabled = true;
        }

        if (CurrentUpgrade == Upgrades.IsGold)
        {
            // Change Material Rocks, Water, Cursor and FX Bling Bling 
        }
    }

    private void Level16()
    {
        if (CurrentUpgrade == Upgrades.IsIceberg)
        {
            _level15Img_MouthIce.enabled = true;
            _icebergs.SetActive(false);
        }

        if (CurrentUpgrade == Upgrades.IsIcePig)
        {
            _level15Img_MouthIce.enabled = false;
            _icebergs.SetActive(true);
        }
    }
}