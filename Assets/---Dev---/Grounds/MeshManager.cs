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
    [Header("Texture")] [SerializeField] private MeshRenderer[] _mesh;
    [SerializeField] private Texture2D _textureBase;
    [SerializeField] private Texture2D _textureSelected;
    [SerializeField] private Texture2D _textureBored;
    private List<Material> _mat;
    private Material _matt;
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
                GetComponentInParent<CrystalsGround>().ChangeCrystal(_crystals[randomNumber]);
        }

        _props[randomNumber].SetActive(true);
        _matt = _mesh[0].material;
    }


    private void Start()
    {
        // foreach (var mesh in _mesh)
        // {
        //     _mat.Add(mesh.material);
        // }
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

    private void Update()
    {
        // _mat.SetTexture("_BaseMap", _textureBase);
        // _mat.SetTexture("Texture", _texture);
    }

    public void UpdateTexture(TileState state, bool isReset)
    {
        // if (state)
        // {
        //     foreach (var mat in _mat)
        //     {
        //         mat.SetTexture("_BaseMap", _textureSelected);
        //     }
        // }
        // else
        // {
        //     foreach (var mat in _mat)
        //     {
        //         mat.SetTexture("_BaseMap", _textureBase);
        //     }
        // }
        
        if (_matt == null) return;

        if (_currentTileState == TileState.Bored && !isReset) return;        


        if (state == TileState.Selected)
            _matt.SetTexture("_BaseMap", _textureSelected);
        else if (state == TileState.Normal)
            _matt.SetTexture("_BaseMap", _textureBase);
        else if (state == TileState.Bored)
        {
            _matt.SetTexture("_BaseMap", _textureBored);
        }

        _currentTileState = state;
    }

}