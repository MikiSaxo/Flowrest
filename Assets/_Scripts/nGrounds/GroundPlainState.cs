using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPlainState : GroundBaseState
{
    public override void InitState(GroundStateManager ground)
    {
        ground.ChangeValues(50, 20);
    }
    public override void EnterState(GroundStateManager ground)
    {
        Debug.Log("Plain");
        ground.ChangeMesh(0);
    }
    
    public override void UpdateState(GroundStateManager ground)
    {
        
    }
}
