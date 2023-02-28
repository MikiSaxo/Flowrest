using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSnowState : GroundBaseState
{
    public override void InitState(GroundStateManager ground)
    {
        //ground.ChangeValues(100, 20);
    }
    
    public override void EnterState(GroundStateManager ground)
    {
        // Debug.Log("Water");
        // ground.ChangeMaterials(2);
        ground.ChangeMesh(6);
        ground.IdOfBloc = 6;
    }
    public override void UpdateState(GroundStateManager ground)
    {
        
    }
    
    public override void CheckUpdate(GroundStateManager ground, GroundBaseState neighboorGround)
    {
    
    }
}
