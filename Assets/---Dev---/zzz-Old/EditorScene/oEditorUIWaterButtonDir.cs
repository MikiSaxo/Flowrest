using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oEditorUIWaterButtonDir : MonoBehaviour
{
    public bool[] WhichDir;
    [SerializeField] private GameObject[] _dir;
    
    private void Start()
    {
        for (int i = 0; i < WhichDir.Length; i++)
        {
            _dir[i].SetActive(WhichDir[i]);
        }
    }
}
