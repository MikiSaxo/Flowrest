using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTextModifier : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<DialogPrefab>().Init(String.Empty, 0); 

    }
}
