using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class GroundPlainsState : GroundBaseState
{
    public override void EnterState(GroundStateManager ground)
    {
        Debug.Log("Plains");
    }
    
    public override void UpdateState(GroundStateManager ground)
    {
        
    }
}
