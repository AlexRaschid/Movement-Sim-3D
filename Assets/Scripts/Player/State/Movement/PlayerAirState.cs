using UnityEngine;

public class PlayerAirState : PlayerBaseState
{
    public override void EnterState(PlayerMovementStateManager player)
    {
        Debug.Log("Hello this is zoidberg from PlayerAirState!");
        
    }

    public override void UpdateState(PlayerMovementStateManager player)
    {
        /*
        player.playerMovement.ApplyCustomGravity();
        player.playerMovement.AirStrafe(player.playerMovement.moveDirection, player.playerMovement.moveSpeed);
        */

        //Debug.Log(player.playerMovement.GetIsGrounded());

        if(player.playerMovement.GetGrounded())
        {
            player.SwitchState(player.stillState);
        }
    }

    public override void OnCollisionEnter(PlayerMovementStateManager player)
    {

    }
}
