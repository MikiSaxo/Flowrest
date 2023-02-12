using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDesertState : GroundBaseState
{
    public override void InitState(GroundStateManager ground)
    {
        // ground.ChangeValues(0, 35);
    }
    public override void EnterState(GroundStateManager ground)
    {
        // Debug.Log("Desert");
        ground.ChangeMesh(1);
        ground.IdOfBloc = 1;
    }
    
    public override void UpdateState(GroundStateManager ground)
    {
        
    }
    
    public override void CheckUpdate(GroundStateManager ground, GroundBaseState neighboorGround)
    {
        switch (neighboorGround)
        {
            case GroundPlainState:
                ground.ChangeState(AllStates.Savanna);
                break;
            case GroundDesertState:
                // ground.ChangeState(1);
                Debug.Log("Deja un Desert");
                break;
            case GroundWaterState:
                ground.ChangeState(AllStates.Geyser);
                break;
            case GroundTropicalState:
                ground.ChangeState(AllStates.Savanna);
                break;
            case GroundSavannaState:
                ground.ChangeState(AllStates.Savanna);
                break;
            case GroundGeyserState:
                ground.ChangeState(AllStates.Geyser);
                break;
        }
    }
}
