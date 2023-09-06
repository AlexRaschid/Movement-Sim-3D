using UnityEngine;

public class PlayerStillState : PlayerBaseState
{
    public override void EnterState(PlayerMovementStateManager player)
    {
        Debug.Log("Hello this is zoidberg from PlayerStillState!");
    }

    public override void UpdateState(PlayerMovementStateManager player)
    {
        //Debug.Log(player.playerMovement.GetGrounded());
        //Debug.Log(Input.GetKey(player.jumpKey));
        if(!player.playerMovement.GetGrounded())
        {
            player.SwitchState(player.airState);
        }
        else if (player.playerMovement.GetGrounded() && Input.GetKey(player.jumpKey))
        {
            player.SwitchState(player.jumpState);
        }

        if(player.moveDirection.x != 0 || player.moveDirection.y != 0)
        {
            player.SwitchState(player.walkingState);
        }
    }

    public override void OnCollisionEnter(PlayerMovementStateManager player)
    {
        
    }

}
