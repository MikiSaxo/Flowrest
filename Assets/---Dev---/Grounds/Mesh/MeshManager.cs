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
    // [SerializeField] private Color _boredColor;

    [Header("Variations")] [SerializeField]
    private MeshVariations[] _meshVariations;

    private Material[] _matt = new Material[2];
    private Material[] _propsMat;
    private Animator[] _propsAnim;
    private Color[] _propsStartColor = new Color[0];
    private TileState _currentTileState;
    private GameObject _currentCrystal;
    private int _randomNb;

    private void Awake()
    {
        if (_meshVariations.Length == 0) return;

        DeactivateAllProps8Crystal();

        int randomNumber = 0;
        randomNumber = Random.Range(0, _meshVariations.Length);
        _randomNb = randomNumber;

        if (_meshVariations.Length > 0)
        {
            _currentCrystal = _meshVariations[randomNumber].Crystals;
            if (GetComponentInParent<CrystalsGround>() != null)
            {
                GetComponentInParent<CrystalsGround>().ChangeCrystal(_meshVariations[randomNumber].Crystals);
            }
        }

        _meshVariations[randomNumber].Props.SetActive(true);

        if (_supportMesh.Length > 0)
        {
            _matt[0] = _supportMesh[0].material;
            _matt[1] = _supportMesh[1].material;
        }


        if (_meshVariations.Length > 0)
        {
            _propsMat = _meshVariations[randomNumber].Props.GetComponent<StockProps>().GetProps();
            _propsAnim = _meshVariations[randomNumber].Props.GetComponent<StockProps>().GetPropsAnim();

            if (_propsMat == null) return;

            _propsStartColor = new Color[_propsMat.Length];

            for (int i = 0; i < _propsMat.Length; i++)
            {
                _propsStartColor[i] = _propsMat[i].GetColor("_BaseColor");
            }
        }

        DestroyOtherObjects(randomNumber);
    }

    public void UpdateCrystal(bool state)
    {
        _currentCrystal.SetActive(state);
    }

    private void DeactivateAllProps8Crystal()
    {
        foreach (var variation in _meshVariations)
        {
            variation.Props.SetActive(false);
            variation.Crystals.SetActive(false);
        }
    }

    public void UpdateTexture(TileState state, bool isReset)
    {
        if (_matt == null) return;

        if (_currentTileState == TileState.Bored && !isReset) return;


        // If tile is selected
        if (state == TileState.Selected)
        {
            if (_supportMesh.Length > 0)
            {
                _matt[0].SetTexture("_BaseMap", _textureSelected);
                _matt[1].SetTexture("_BaseMap", _textureBase);

                // Launch anim for current props when tile selected
                foreach (var prop in _propsAnim)
                {
                    prop.SetTrigger("Launch");
                }
            }
        }
        // If tile is Normal
        else if (state == TileState.Normal)
        {
            if (_supportMesh.Length > 0)
            {
                _matt[0].SetTexture("_BaseMap", _textureBase);
                _matt[1].SetTexture("_BaseMap", _textureBase);
            }
        }

        // Update current state
        _currentTileState = state;
    }

    private void CheckTileColor()
    {
        // if (_propsStartColor.Length == 0) return;
        //
        // if (_currentTileState == TileState.Bored)
        // {
        //     for (int i = 0; i < _propsMat.Length; i++)
        //     {
        //         bool hasTexture = _propsMat[i].GetTexture("_BaseMap");
        //         if (!hasTexture)
        //         {
        //             //_propsMat[i].SetColor("_BaseColor", _boredColor);
        //         }
        //         else
        //         {
        //             _propsMat[i].SetTexture("_BaseMap", _textureBored);
        //         }
        //     }
        // }
        // else
        // {
        //     for (int i = 0; i < _propsMat.Length; i++)
        //     {
        //         bool hasTexture = _propsMat[i].GetTexture("_BaseMap");
        //         if (!hasTexture)
        //         {
        //             _propsMat[i].SetColor("_BaseColor", _propsStartColor[i]);
        //         }
        //         else
        //         {
        //             _propsMat[i].SetTexture("_BaseMap", _textureBase);
        //         }
        //     }
        // }
    }

    public GameObject GetSpecificParticule()
    {
        return _meshVariations[_randomNb].Particules;
    }

    private void DestroyOtherObjects(int indexToNotDestroy)
    {
        for (int i = 0; i < _meshVariations.Length; i++)
        {
            if(i == indexToNotDestroy) continue;
            
            if(_meshVariations[i].Props != null)
                Destroy(_meshVariations[i].Props);
            if(_meshVariations[i].Crystals != null)
                Destroy(_meshVariations[i].Crystals);
        }
    }
}