using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FollowMouseDND : MonoBehaviour
{
    public static FollowMouseDND Instance;
    
    public bool CanMove;
    [SerializeField] private Image _iconButton;
    [SerializeField] private TextMeshProUGUI _text;


    private void Awake()
    {
        Instance = this;
    }

    public void Move()
    {
        var mousePos = Input.mousePosition;
        gameObject.transform.position = mousePos;
    }

    private void Update()
    {
        if(CanMove)
            Move();
    }

    public void UpdateObject(Color color, string text)
    {
        _iconButton.color = color;
        _text.text = text;
    }
}
