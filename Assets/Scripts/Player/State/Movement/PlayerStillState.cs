using UnityEngine;

public class PlayerStillState : PlayerBaseState
{
    public override void EnterState(PlayerMovementStateManager player)
    {
        Debug.Log("Hello this is zoidberg from PlayerStillState!Movement");
    }

    public override void UpdateState(PlayerMovementStateManager player)
    {

        if(Mathf.Round(player.moveDirection.magnitude) != 0 
            || Mathf.Round(player.rb.velocity.magnitude) != 0)
        {
            player.SwitchState(player.walkingState);
        } 
        else if(Input.GetKey(player.jumpKey) && player.playerMovement.canJumpCast)
        {
            player.SwitchState(player.jumpState);
        }
    }

    public override void OnCollisionEnter(PlayerMovementStateManager player)
    {
        
    }

}
