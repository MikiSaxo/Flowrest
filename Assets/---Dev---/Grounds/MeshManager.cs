using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] _mesh;
    [SerializeField] private Texture2D _textureBase;
    [SerializeField] private Texture2D _textureSelected;
    private List<Material> _mat;
    private Material _matt;

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