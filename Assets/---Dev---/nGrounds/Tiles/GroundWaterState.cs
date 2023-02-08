using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundWaterState : GroundBaseState
{
    public override void InitState(GroundStateManager ground)
    {
        //ground.ChangeValues(100, 20);
    }
    
    public override void EnterState(GroundStateManager ground)
    {
        // Debug.Log("Water");
        // ground.ChangeMaterials(2);
        ground.ChangeMesh(2);
        ground.IdOfBloc = 2;
    }
    public override void UpdateState(GroundStateManager ground)
    {
        
    }
    
    public override void CheckUpdate(GroundStateManager ground, GroundBaseState neighboorGround)
    {
        switch (neighboorGround)
        {
            case GroundPlainState:
                ground.ChangeState(AllStates.Tropical);
                break;
            case GroundDesertState:
                ground.ChangeState(AllStates.HotSpring);
                break;
            case GroundWaterState:
                Debug.Log("deja de l'eau");
                break;
            case GroundTropicalState:
                ground.ChangeState(AllStates.Tropical);
                break;
            case GroundSavaneState:
                ground.ChangeState(AllStates.Plain);
                break;
            case GroundHotSpringState:
                ground.ChangeState(AllStates.HotSpring);
                break;
        }
    }
}
