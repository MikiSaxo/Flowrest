using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSavannaState : GroundBaseState
{
    public override void InitState(GroundStateManager ground)
    {
        // ground.ChangeValues(0, 35);
    }
    public override void EnterState(GroundStateManager ground)
    {
        ground.ChangeMesh(4);
        ground.IdOfBloc = 4;
    }
    
    public override void UpdateState(GroundStateManager ground)
    {
        
    }
    
    public override void CheckUpdate(GroundStateManager ground, GroundBaseState neighboorGround)
    {
        switch (neighboorGround)
        {
            case GroundPlainState:
                ground.ChangeState(AllStates.Plain);
                break;
            case GroundDesertState:
                ground.ChangeState(AllStates.Desert);
                break;
            case GroundWaterState:
                ground.ChangeState(AllStates.Plain);
                break;
            case GroundTropicalState:
                ground.ChangeState(AllStates.Plain);
                break;
            case GroundSavannaState:
                // ground.ChangeState(4);
                Debug.Log("Deja une Savane");
                break;
            case GroundGeyserState:
                ground.ChangeState(AllStates.Tropical);
                break;
        }
    }
}
