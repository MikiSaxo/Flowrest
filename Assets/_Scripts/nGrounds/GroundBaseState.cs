using UnityEngine;

public abstract class GroundBaseState
{
    public abstract void EnterState(GroundStateManager ground);
    
    public abstract void UpdateState(GroundStateManager ground);
    
}
