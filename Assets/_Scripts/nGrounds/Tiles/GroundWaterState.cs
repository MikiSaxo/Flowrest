using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundWaterState : GroundBaseState
{
    public override void InitState(GroundStateManager ground)
    {
        ground.ChangeValues(100, 20);
    }
    
    public override void EnterState(GroundStateManager ground)
    {
        Debug.Log("Water");
        // ground.ChangeMaterials(2);
        ground.ChangeMesh(2);
        ground.IdOfBloc = 2;
    }   
        

    
    public override void UpdateState(GroundStateManager ground)
    {
        
    }
}
