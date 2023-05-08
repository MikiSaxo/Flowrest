using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeshManager : MonoBehaviour
{
    [Header("Texture")] [SerializeField] private MeshRenderer[] _mesh;
    [SerializeField] private Texture2D _textureBase;
    [SerializeField] private Texture2D _textureSelected;
    private List<Material> _mat;
    private Material _matt;

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

    private void Start()
    {
        // foreach (var mesh in _mesh)
        // {
        //     _mat.Add(mesh.material);
        // }
        _matt = _mesh[0].material;
    }

    private void Update()
    {
        // _mat.SetTexture("_BaseMap", _textureBase);
        // _mat.SetTexture("Texture", _texture);
    }

    public void UpdateTexture(bool state)
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


        if (state)
            _matt.SetTexture("_BaseMap", _textureSelected);
        else
            _matt.SetTexture("_BaseMap", _textureBase);
    }
}