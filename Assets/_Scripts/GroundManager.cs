using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    private Vector3 _startPos;

    private void OnTriggerEnter(Collider other)
    {
        OnEntered();
    }

    private void OnTriggerExit(Collider other)
    {
        OnLeaved();
    }

    private void Start()
    {
        _startPos = transform.position;
    }

    private void OnSelected()
    {
        transform.DOMoveY(transform.position.y + 2, .5f);
    }

    private void OnEntered()
    {
        transform.DOMoveY(transform.position.y + .2f, .5f);
    }

    private void OnLeaved()
    {
        transform.DOMove(_startPos, .5f);
    } 
}
