using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDesertState : GroundBaseState
{
    public override void InitState(GroundStateManager ground)
    {
        ground.ChangeValues(0, 35);
    }
    public override void EnterState(GroundStateManager ground)
    {
        Debug.Log("Desert");
        //ground.ChangeMaterials(1);
        ground.ChangeMesh(1);
        ground.IDofBloc = 1;
    }
    
    public override void UpdateState(GroundStateManager ground)
    {
        
    }
}
