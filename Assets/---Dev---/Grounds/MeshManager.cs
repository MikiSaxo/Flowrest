using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TileState
{
    Normal,
    Selected,
    Bored
}

public class MeshManager : MonoBehaviour
{
    [Header("Texture")] [SerializeField] private MeshRenderer[] _supportMesh;
    [SerializeField] private Texture2D _textureBase;
    [SerializeField] private Texture2D _textureSelected;
    [SerializeField] private Texture2D _textureBored;
    [SerializeField] private Color _boredColor;

    [Header("Props")] [SerializeField] private StockProps[] _stockProps;

    private Material[] _matt = new Material[2];
    private Material[] _propsMat;
    private Color[] _propsStartColor = new Color[0];
    private TileState _currentTileState;

    [Header("Variations")] [SerializeField]
    private GameObject[] _props;

    [SerializeField] private GameObject[] _crystals;


    private void Awake()
    {
        if (_crystals.Length == 0 && _props.Length == 0) return;

        DeactivateAllProps8Crystal();

        int randomNumber = 0;
        randomNumber = Random.Range(0, _crystals.Length != 0 ? _crystals.Length : _props.Length);

        if (_crystals.Length > 0)
        {
            if (GetComponentInParent<CrystalsGround>() != null)
            {
                GetComponentInParent<CrystalsGround>().ChangeCrystal(_crystals[randomNumber]);
            }
        }

        _props[randomNumber].SetActive(true);
        _matt[0] = _supportMesh[0].material;
        _matt[1] = _supportMesh[1].material;


        if (_stockProps.Length > 0)
        {
            _propsMat = _stockProps[randomNumber].GetProps();
            _propsStartColor = new Color[_propsMat.Length];

            for (int i = 0; i < _propsMat.Length; i++)
            {
                _propsStartColor[i] = _propsMat[i].GetColor("_BaseColor");
            }
        }
    }

    private void DeactivateAllProps8Crystal()
    {
        foreach (var crystal in _crystals)
        {
            crystal.SetActive(false);
        }

        foreach (var prop in _props)
        {
            prop.SetActive(false);
        }
    }

    public void UpdateTexture(TileState state, bool isReset)
    {
        if (_matt == null) return;

        if (_currentTileState == TileState.Bored && !isReset) return;


        if (state == TileState.Selected)
        {
            _matt[0].SetTexture("_BaseMap", _textureSelected);
            _matt[1].SetTexture("_BaseMap", _textureBase);
        }
        else if (state == TileState.Normal)
        {
            _matt[0].SetTexture("_BaseMap", _textureBase);
            _matt[1].SetTexture("_BaseMap", _textureBase);
        }
        else if (state == TileState.Bored)
        {
            _matt[0].SetTexture("_BaseMap", _textureBored);
            _matt[1].SetTexture("_BaseMap", _textureBored);
        }
        
        _currentTileState = state;
        CheckTileColor();
    }

    private void CheckTileColor()
    {
        if (_propsStartColor.Length == 0) return;

        if (_currentTileState == TileState.Bored)
        {
            for (int i = 0; i < _propsMat.Length; i++)
            {
                bool hasTexture = _propsMat[i].GetTexture("_BaseMap");
                if (!hasTexture)
                {
                    _propsMat[i].SetColor("_BaseColor", _boredColor);
                }
                else
                {
                    _propsMat[i].SetTexture("_BaseMap", _textureBored);
                }
            }
        }
        else
        {
            for (int i = 0; i < _propsMat.Length; i++)
            {
                bool hasTexture = _propsMat[i].GetTexture("_BaseMap");
                if (!hasTexture)
                {
                    _propsMat[i].SetColor("_BaseColor", _propsStartColor[i]);
                }
                else
                {
                    _propsMat[i].SetTexture("_BaseMap", _textureBase);
                }
            }
        }
    }
}