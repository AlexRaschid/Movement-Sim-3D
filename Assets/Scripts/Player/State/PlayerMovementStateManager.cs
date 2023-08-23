using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovementStateManager : MonoBehaviour
{
    PlayerBaseState currentState; //will hold reference to current state, only 1 at a time
    public PlayerAirState airState = new PlayerAirState();
    public PlayerCrouchingState crouchingState = new PlayerCrouchingState();
    public PlayerStillState stillState = new PlayerStillState();
    public PlayerWalkingState walkingState = new PlayerWalkingState();
    public PlayerJumpState jumpState = new PlayerJumpState();

    [Header("Keybinds and Input")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C;
    public float horizontalInput;
    public float verticalInput;

    [Header("Movement Logic")]
    public BunnyHopRigidbody playerMovement;
    public Transform orientation;
    public Vector3 moveDirection;
    public Rigidbody rb;

    [Header("UI Manager")]
    private UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        currentState = stillState;
        currentState.EnterState(this); //'this' refers to the context (this monobehaviour script)


        rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<BunnyHopRigidbody>();
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();


        //playerMovement.SetReadyToJump(true);
        //rb.useGravity = false; //we'll make our own!
        /*Freezing rotation is not necessary, but highly recommended
        if we're making a character rather than just some object
        that happens to be on a slope.*/
        //rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        uiManager.InfoReport(playerMovement.velocity.magnitude.ToString(), currentState.ToString());

        //Best practice indicates to keep input data in update, which also includes the camera
        horizontalInput = Input.GetAxisRaw("Horizontal"); //a+d
        verticalInput = Input.GetAxisRaw("Vertical"); //w+s
        //playerMovement.SetCanJumpCast();

        playerMovement.GetIsGrounded();

        moveDirection = (transform.forward * Input.GetAxis("Vertical") +
                         transform.right * Input.GetAxis("Horizontal")).normalized;


        currentState.UpdateState(this);
    }

    void FixedUpdate()
    {
        
        //playerMovement.ObeyGravity();
        playerMovement.SimulatePhysics();

/*
        // Update velocity with external forces
        Vector3 externalForces = Vector3.zero; // Calculate any external forces here
        playerMovement.velocity += externalForces * Time.fixedDeltaTime;
*/
        // Apply velocity change
        rb.MovePosition(rb.position + playerMovement.velocity * Time.fixedDeltaTime);

        MovePlayer();
    }

    public void SwitchState(PlayerBaseState state)
    {
        currentState = state;

        //why use state as opposed to currentState?
        state.EnterState(this);
    }

    public void MovePlayer()
    {

        if (currentState == airState)
        {
            //playerMovement.ApplyCustomGravity();
            playerMovement.AirStrafe(moveDirection, playerMovement.moveSpeed);
        }

        playerMovement.SetOnSlope();

        if (playerMovement.GetOnSlope())
        {
            playerMovement.ApplySlopeGravity(playerMovement.slopeNormal);
        }

        // Apply friction when on ground
        if (playerMovement.GetIsGrounded())
        {
            Vector3 frictionForce = -playerMovement.velocity.normalized * playerMovement.groundFriction * Time.fixedDeltaTime;
            playerMovement.velocity += frictionForce;
        }


        /*
        else
        {
            onSlope = playerMovement.CheckSlope(out slopeNormal);

            // Apply slope gravity adjustment
            if (onSlope)
            {
                ApplySlopeGravity(slopeNormal);
                MoveOnGround(moveDirection, moveSpeed, groundFriction);
            }
            else
            {
                MoveOnGround(moveDirection, moveSpeed, groundFriction);
            }
        }
        */
    }
    /*
    public void MovePlayer()
    {
        //Debug.Log(currentState == jumpState);
        //Debug.Log(playerMovement.canJumpCast);
        
        //calc movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        //Debug.Log(moveDirection);

        //on slope
        if(currentState == walkingState &&
            playerMovement.OnSlope() && !playerMovement.GetExitingSlope())
        {
            
            rb.velocity = playerMovement.MoveGround(playerMovement.GetSlopeMoveDirection(moveDirection), rb.velocity);
            
        }

        //walking
        else if(currentState == walkingState 
                && playerMovement.GetGrounded())
        {
            rb.velocity = playerMovement.MoveGround(moveDirection, rb.velocity);
        }//perform Jump
        else if(currentState == jumpState)
        {
            playerMovement.SetReadyToJump(false);
            playerMovement.Jump();
            playerMovement.Invoke(nameof(playerMovement.ResetJump), playerMovement.GetJumpCoolDown());
        }
        //in air
        else if(!playerMovement.GetGrounded())
            playerMovement.MoveAir(moveDirection, rb.velocity);
    }
    */
}
