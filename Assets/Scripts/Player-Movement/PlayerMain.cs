using System.Collections;
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
        that happens to be on a slope.*/
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
        //Mode - Standing
        if(playerMovement.GetGrounded() && rb.velocity.magnitude == 0)
        {
            state = PlayerState.standing;
        }
        //Mode - CrouchingHold + Air
        else if(Input.GetKey(crouchKey) && rb.velocity.y != 0)
        {
            state = PlayerState.crouchingHoldInAir;
        }
        //Mode - crouchingHop
        else if(Input.GetKey(crouchKey) && Input.GetKeyDown(jumpKey))
        {
            state = PlayerState.crouchingHop;
        }
        //Mode - CrouchingHold
        else if(Input.GetKey(crouchKey))
        {
            state = PlayerState.crouchingHold;
        }
        //Mode - CrouchingRelease
        else if(Input.GetKeyUp(crouchKey))
        {
            state = PlayerState.crouchingRelease;
        }
        
        //Mode - Jump
        else if(Input.GetKey(jumpKey) && playerMovement.canJumpCast)
        {
            state = PlayerState.jump;
        }
        //Mode - Walking
        else if(playerMovement.GetGrounded() && rb.velocity.magnitude != 0)
        {
            state = PlayerState.walking;
        }
        //Mode - Air
        else
        {
            state = PlayerState.air;
        }
    }

    private void MyInput()
    {

        //when to jump
        if(state == PlayerState.jump || state == PlayerState.crouchingHop || state == PlayerState.standing && //crouchJumping
            playerMovement.GetReadyToJump() && 
            playerMovement.canJumpCast )//&& playerMovement.canJumpCast playerMovement.GetGrounded()
        {
            playerMovement.SetReadyToJump(false);

            playerMovement.Jump();

            playerMovement.Invoke(nameof(playerMovement.ResetJump), playerMovement.GetJumpCoolDown());
        }

        //start crouch
        if(state == PlayerState.crouchingHold)
        {
            Debug.Log("Perform Crouch Down!");
            playerMovement.Crouch();
            //ToDo: Re Implement Crouching
            
            /*
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            */
        }

        //stop crouch
        if(state == PlayerState.crouchingRelease)
        {
            Debug.Log("Perform Crouch Up!");
            playerMovement.Stand();
            
            //transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
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
