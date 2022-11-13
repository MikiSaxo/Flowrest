using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorWaterFlow : MonoBehaviour
{
    [SerializeField] private bool[] _whichDirections;
    [SerializeField] private GameObject[] _directions;
    
    void Start()
    {
        for (int i = 0; i < _whichDirections.Length; i++)
        {
            _directions[i].SetActive(_whichDirections[i]);
        }
    }
}
