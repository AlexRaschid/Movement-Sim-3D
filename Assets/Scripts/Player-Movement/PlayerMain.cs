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
    

    public PlayerState state;
    public enum PlayerState 
    {
        walking,
        sprinting,
        crouching,
        sliding,
        air
    }


    void Start()
    {
        rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerSliding = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSliding>();
    }

    
    void Update()
    {
        //Best practice indicates to keep input data in update, which also includes the camera
        horizontalInput = Input.GetAxisRaw("Horizontal"); //a+d
        verticalInput = Input.GetAxisRaw("Vertical"); //w+s
        
        
        StateHandler();

    }

    //FixedUpdate should be used instead of Update when dealing with Rigidbody.
    void FixedUpdate()
    {
        playerMovement.ObeyGravity();
        MovePlayer();
        //Standards indicate you track input in Update(),
        //however in this case you are currently calculating playerMovement with Crouch, Jump, Stand
        //so right now its alright, but seperate these to keep the convention
        MyInput();

        if(playerMovement.sliding)
            playerSliding.SlidingMovement();
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

    private void MyInput()
    {

        //when to jump
        if(Input.GetKey(jumpKey) && playerMovement.GetReadyToJump() && playerMovement.canJumpCast )//&& playerMovement.canJumpCast playerMovement.GetGrounded()
        {
            playerMovement.SetReadyToJump(false);

            playerMovement.Jump();

            playerMovement.Invoke(nameof(playerMovement.ResetJump), playerMovement.GetJumpCoolDown());
        }

        //start crouch
        if(Input.GetKeyDown(crouchKey))
        {
            Debug.Log("Perform Crouch Down!");
            playerMovement.Crouch();
            //ToDo: Re Implement Crouching
            
            /*
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            */
        }

        //stop crouch
        if(Input.GetKeyUp(crouchKey))
        {
            Debug.Log("Perform Crouch Up!");
            playerMovement.Stand();
            
            //transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        //Mode - crouching
        if(Input.GetKey(crouchKey))
        {
            state = PlayerState.crouching;
        }
        //Mode - walking
        else if(playerMovement.GetGrounded())
        {
            state = PlayerState.walking;
        }
        //Mode - sprinting
        if(playerMovement.GetGrounded() && Input.GetKey(sprintKey))
        {
            state = PlayerState.sprinting;
        }
        //Mode - Air
        else
        {
            state = PlayerState.air;
        }
    }

    

}
