using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance;
    
    [SerializeField] private GameObject _parent;
    [SerializeField] private Image _image;

    private void Awake()
    {
        Instance = this;
    }

    public void InitPopUp(Sprite[] sprites)
    {
        GetComponent<LegendScroll>().InitLegend(sprites);
    }


    public void UpdatePopUp(bool state)
    {
        _parent.SetActive(state);
    }
}
