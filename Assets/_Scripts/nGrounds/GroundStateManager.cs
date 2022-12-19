using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundStateManager : MonoBehaviour
{
    public GroundBaseState currentState;
    public GroundPlainsState PlainsState = new GroundPlainsState();
    public GroundDesertState DesertState = new GroundDesertState();
    public GroundWaterState WaterState = new GroundWaterState();

    [SerializeField] private MeshRenderer _groundMaterial;
    [SerializeField] protected Material[] _materials;

    private void Start()
    {
        currentState = PlainsState;
        currentState.EnterState(this);
    }

    public void ChangeMaterials(int materialNb)
    {
        _groundMaterial.material = _materials[materialNb];
    }
}
