using UnityEngine;

public class PlayerWalkingState : PlayerBaseState
{
    public override void EnterState(PlayerMovementStateManager player)
    {
        Debug.Log("Hello this is zoidberg from PlayerWalkingState!");
        
    }

    public override void UpdateState(PlayerMovementStateManager player)
    {

        if(Mathf.Round(player.rb.velocity.magnitude) == 0)
        {
            player.SwitchState(player.stillState);
        }
        else if(Input.GetKey(player.jumpKey) )//&& player.playerMovement.canJumpCast*
        {
            player.SwitchState(player.jumpState);
        }
    }

    public override void OnCollisionEnter(PlayerMovementStateManager player)
    {
        
    }

}
