using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public override void EnterState(PlayerMovementStateManager player)
    {
        Debug.Log("Hello this is zoidberg from PlayerJumpState!");
    }

    public override void UpdateState(PlayerMovementStateManager player)
    {
        
        if(!player.playerMovement.GetCanJumpCast())
        {
            player.SwitchState(player.airState);
        }
        /*else if(Mathf.Round(player.rb.velocity.magnitude) != 0)
        {
            player.SwitchState(player.walkingState);
        } 
        */
    }

    public override void OnCollisionEnter(PlayerMovementStateManager player)
    {
        
    }
}
