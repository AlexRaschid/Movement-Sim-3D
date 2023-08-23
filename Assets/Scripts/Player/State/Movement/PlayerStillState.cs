using UnityEngine;

public class PlayerStillState : PlayerBaseState
{
    public override void EnterState(PlayerMovementStateManager player)
    {
        Debug.Log("Hello this is zoidberg from PlayerStillState!");
    }

    public override void UpdateState(PlayerMovementStateManager player)
    {
        Debug.Log(player.playerMovement.GetIsGrounded());
        Debug.Log(Input.GetKey(player.jumpKey));
        if(!player.playerMovement.GetIsGrounded())
        {
            player.SwitchState(player.airState);
        }
        if (player.playerMovement.GetIsGrounded() && Input.GetKeyDown(player.jumpKey))
        {
            player.SwitchState(player.jumpState);
        }
    }

    public override void OnCollisionEnter(PlayerMovementStateManager player)
    {
        
    }

}
