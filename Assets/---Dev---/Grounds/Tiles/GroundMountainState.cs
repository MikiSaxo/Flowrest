using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMountainState : GroundBaseState
{
    public override void InitState(GroundStateManager ground)
    {
        //ground.ChangeValues(100, 20);
    }
    
    public override void EnterState(GroundStateManager ground)
    {
        // Debug.Log("Mountain");
        // ground.ChangeMaterials(2);
        ground.ChangeMesh(10);
        ground.IdOfBloc = 10;
    }
    public override void UpdateState(GroundStateManager ground)
    {
        
    }
    
    public override void CheckUpdate(GroundStateManager ground, GroundBaseState neighboorGround)
    {
       
    }
}
