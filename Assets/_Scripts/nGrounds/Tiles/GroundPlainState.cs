using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPlainState : GroundBaseState
{
    public override void InitState(GroundStateManager ground)
    {
        //ground.ChangeValues(15, 15);
    }
    public override void EnterState(GroundStateManager ground)
    {
        // Debug.Log("Plain");
        ground.ChangeMesh(0);
        ground.IdOfBloc = 0;
        //ground.GetComponentInChildren<PlainMesh>().EnterState();
    }
    
    public override void UpdateState(GroundStateManager ground)
    {
        
    }
    public override void CheckUpdate(GroundStateManager ground, GroundBaseState neighboorGround)
    {
        switch (neighboorGround)
        {
            case GroundPlainState:
                Debug.Log("Deja une plaine");
                break;
            case GroundDesertState:
                ground.ChangeState(AllStates.Savane);
                break;
            case GroundWaterState:
                ground.ChangeState(AllStates.Tropical);
                break;
            case GroundTropicalState:
                ground.ChangeState(AllStates.Tropical);
                break;
            case GroundSavaneState:
                ground.ChangeState(AllStates.Savane);
                break;
            case GroundHotSpringState:
                ground.ChangeState(AllStates.Tropical);
                break;
        }
    }
}
