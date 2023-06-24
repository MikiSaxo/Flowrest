using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorFollow : MonoBehaviour
{
    void FixedUpdate()
    {
        gameObject.transform.position = Input.mousePosition;
    }
}
