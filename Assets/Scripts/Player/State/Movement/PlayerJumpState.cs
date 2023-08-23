using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public override void EnterState(PlayerMovementStateManager player)
    {
        Debug.Log("Hello this is zoidberg from PlayerJumpState!");
        player.playerMovement.Jump();
    }

    public override void UpdateState(PlayerMovementStateManager player)
    {
        /*
        if(Mathf.Round(player.rb.velocity.magnitude) == 0)
        {
            player.SwitchState(player.stillState);
        }
        else if(Mathf.Round(player.rb.velocity.magnitude) != 0)
        {
            player.SwitchState(player.walkingState);
        } 
        */
    }

    public override void OnCollisionEnter(PlayerMovementStateManager player)
    {
        
    }
}
