using UnityEngine;

public abstract class PlayerBaseState
{
    public abstract void EnterState(PlayerMovementStateManager player);

    public abstract void UpdateState(PlayerMovementStateManager player);

    public abstract void OnCollisionEnter(PlayerMovementStateManager player);

}
