using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPlainState : GroundBaseState
{
    public override void InitState(GroundStateManager ground)
    {
        ground.ChangeValues(15, 15);
    }
    public override void EnterState(GroundStateManager ground)
    {
        Debug.Log("Plain");
        ground.ChangeMesh(0);
        ground.IdOfBloc = 0;
        ground.GetComponentInChildren<PlainMesh>().EnterState();
    }
    
    public override void UpdateState(GroundStateManager ground)
    {
        
    }
}
