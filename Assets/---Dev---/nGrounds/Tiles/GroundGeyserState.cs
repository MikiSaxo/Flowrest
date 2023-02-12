using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundGeyserState : GroundBaseState
{
    public override void InitState(GroundStateManager ground)
    {
        // ground.ChangeValues(0, 35);
    }
    public override void EnterState(GroundStateManager ground)
    {
        ground.ChangeMesh(5);
        ground.IdOfBloc = 5;
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
                ground.ChangeState(AllStates.Desert);
                break;
            case GroundWaterState:
                ground.ChangeState(AllStates.Water);
                break;
            case GroundTropicalState:
                ground.ChangeState(AllStates.Water);
                break;
            case GroundSavannaState:
                ground.ChangeState(AllStates.Tropical);
                break;
            case GroundGeyserState:
                // ground.ChangeState(5);
                Debug.Log("Deja une Source Chaude");
                break;
        }
    }
}
