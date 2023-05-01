using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer _mesh;
    [SerializeField] private Texture2D _texture;
    private Material _mat;

    private void Start()
    {
        _mat = _mesh.material;
    }

    private void Update()
    {
        // _mat.SetTexture("Texture", _texture);
    }
}
