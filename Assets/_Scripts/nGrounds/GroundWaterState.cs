using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundWaterState : GroundBaseState
{
    public override void EnterState(GroundStateManager ground)
    {
        Debug.Log("Water");
    }
    
    public override void UpdateState(GroundStateManager ground)
    {
        
    }
}
