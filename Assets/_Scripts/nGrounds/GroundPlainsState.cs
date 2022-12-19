using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPlainsState : GroundBaseState
{
    public override void InitState(GroundStateManager ground)
    {
        ground.ChangeValues(50, 20);
    }
    public override void EnterState(GroundStateManager ground)
    {
        Debug.Log("Plains");
        ground.ChangeMaterials(0);
    }
    
    public override void UpdateState(GroundStateManager ground)
    {
        
    }
}
