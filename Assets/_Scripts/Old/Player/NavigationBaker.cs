using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavigationBaker : MonoBehaviour
{
    public List<NavMeshSurface> surfaces = new List<NavMeshSurface>();

    public static NavigationBaker Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void BuildNavSurface()
    {
        for (int i = 0; i < surfaces.Count; i++)
        {
            surfaces[i].BuildNavMesh();
        }
    }
}