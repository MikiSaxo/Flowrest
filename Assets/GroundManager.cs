using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    private Vector3 _startPos;

    private void Start()
    {
        _startPos = transform.position;
    }

    public void OnSelected()
    {
        transform.DOMoveY(transform.position.y + 1, .5f);
    }

    public void OnEntered()
    {
        print("allo");
        transform.DOMoveY(transform.position.y + .5f, .5f);
    }

    public void OnLeave()
    {
        transform.DOMove(_startPos, .5f);
    } 
}
