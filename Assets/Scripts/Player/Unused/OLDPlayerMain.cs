/*  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Note to self: Try to maintain Camel Casing
//try to keep main logic/state here, and leave calculations to the other methods
public class PlayerMain : MonoBehaviour
{   
    [Header("Keybinds and Input")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C;
    float horizontalInput;
    float verticalInput;

    [Header("Movement Logic")]
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] public PlayerSliding playerSliding;
    public Transform orientation;
    Vector3 moveDirection;
    Rigidbody rb;
    
    [Header("UI Manager")]
    private UIManager uiManager;

    public PlayerState state;
    public enum PlayerState 
    {
        standing,
        walking,
        jump,
        crouchingHold,
        crouchingHoldInAir,
        crouchingRelease,
        crouchingHop,
        sliding,
        air
    }

    void Start()
    {
        rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerSliding = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSliding>();
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        playerMovement.SetReadyToJump(true);
        rb.useGravity = false; //we'll make our own!
        /*Freezing rotation is not necessary, but highly recommended
        if we're making a character rather than just some object
        that happens to be on a slope.8/
        rb.freezeRotation = true;
    }
    void Update()
    {
        uiManager.InfoReport(rb.velocity.magnitude.ToString(), state.ToString());

        playerMovement.SetCanJumpCast();
        StateHandler();
        MyInput();
        //Best practice indicates to keep input data in update, which also includes the camera
        horizontalInput = Input.GetAxisRaw("Horizontal"); //a+d
        verticalInput = Input.GetAxisRaw("Vertical"); //w+s
        
        

    }
    //FixedUpdate should be used instead of Update when dealing with Rigidbody.
    void FixedUpdate()
    {
        playerMovement.ObeyGravity();
        MovePlayer();
        
        if(playerMovement.sliding)
            playerSliding.SlidingMovement();
    }
    private void StateHandler()
    {
        //Check if grounded or able to jump
        if(playerMovement.GetGrounded() || playerMovement.canJumpCast)
        {
            //Mode - CrouchingHold
            if(Input.GetKey(crouchKey))
            {
                //Crouch + Jump
                if(Input.GetKey(jumpKey))
                {
                    state = PlayerState.crouchingHop;
                } 
                else
                {
                    state = PlayerState.crouchingHold;
                }  
            }
            ////Mode - CrouchingRelease
            else if(Input.GetKeyUp(crouchKey))
            {
                state = PlayerState.crouchingRelease;
            }
            //Mode - Jump
            else if(Input.GetKey(jumpKey))
            {
                state = PlayerState.jump;
            }
            //Mode - Standing (idle)
            else if(Mathf.Round(rb.velocity.magnitude) == 0)
            {
                state = PlayerState.standing;
            }
            //Mode - Walking
            else if(rb.velocity.magnitude != 0)
            {
                state = PlayerState.walking;
            }
        } 
        //Mode - Air
        else
        {
            state = PlayerState.air;
        }

        //why did i make this one?
        /*
        //Mode - CrouchingHold + Air
        else if(Input.GetKey(crouchKey) && rb.velocity.y != 0)
        {
            state = PlayerState.crouchingHoldInAir;
        }
        8/  
    }

    private void MyInput()
    {
        switch(state)
        {
            //Check for Jump
            case PlayerState.jump:
            case PlayerState.crouchingHop:
                playerMovement.SetReadyToJump(false);
                playerMovement.Jump();
                playerMovement.Invoke(nameof(playerMovement.ResetJump), playerMovement.GetJumpCoolDown());
                break;
            case PlayerState.crouchingHold:
                playerMovement.Crouch();
                break;
            case PlayerState.crouchingRelease:
                playerMovement.Stand();
                break;      
        }
    }

    public void MovePlayer()
    {
        
        //calc movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //on slope
        if(playerMovement.OnSlope() && !playerMovement.GetExitingSlope())
        {
            
            rb.velocity = playerMovement.MoveGround(playerMovement.GetSlopeMoveDirection(moveDirection), rb.velocity);
            
        }

        //walking
        else if(playerMovement.GetGrounded())
        {
            rb.velocity = playerMovement.MoveGround(moveDirection, rb.velocity);
        }
            //rb.velocity = MoveGround(moveDirection.normalized, rb.velocity);

        //in air
        else if(!playerMovement.GetGrounded())
            playerMovement.MoveAir(moveDirection, rb.velocity);

            
        // turn gravity off while on slope
        
        //rb.useGravity = !OnSlope();
    }

    

}

*/
