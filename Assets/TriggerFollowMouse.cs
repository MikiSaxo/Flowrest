using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFollowMouse : MonoBehaviour
{
    private bool _isGroundEntered;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<GroundIndicator>())
            _isGroundEntered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<GroundIndicator>())
            _isGroundEntered = false;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && !_isGroundEntered)
            MapManager.Instance.ResetBig();
    }   
}