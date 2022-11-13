using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorUIWaterButtonDir : MonoBehaviour
{
    [SerializeField] private bool[] _whichDir;
    [SerializeField] private GameObject[] _dir;
    
    private void Start()
    {
        for (int i = 0; i < _whichDir.Length; i++)
        {
            _dir[i].SetActive(_whichDir[i]);
        }
    }
}
