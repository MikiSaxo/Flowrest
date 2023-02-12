using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTropicalState : GroundBaseState
{
    public override void InitState(GroundStateManager ground)
    {
        // ground.ChangeValues(0, 35);
    }
    public override void EnterState(GroundStateManager ground)
    {
        ground.ChangeMesh(3);
        ground.IdOfBloc = 3;
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
                ground.ChangeState(AllStates.Savanna);
                break;
            case GroundWaterState:
                ground.ChangeState(AllStates.Water);
                break;
            case GroundTropicalState:
                // ground.ChangeState(3);
                Debug.Log("Deja un Tropical");
                break;
            case GroundSavannaState:
                ground.ChangeState(AllStates.Plain);
                break;
            case GroundGeyserState:
                ground.ChangeState(AllStates.Water);
                break;
        }
    }
}
