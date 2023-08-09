using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /*
        Bug Fix / Feature Todo's:
            b-sliding along the middle of a ramp/slope
            b-faster movement speed on slope
            b-not being able to jump running down slope

            b-when landing from a jump, small bounce back on the landing

            b-player 'sticks' to walls/side of objects when applying movement key

            f-proper crouch jumping (jump + crouch)
            b-inital jump when crouched has less height, every jump after retains regular jump height
    */

public class PlayerMovement : MonoBehaviour
{

    //Why is HEader structured like this
    //To for Input varible Headers in Unity
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;    


    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;



    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;


    public MovementState state;
    //What is enum?
    public enum MovementState 
    {
        walking,
        sprinting,
        crouching,
        air
    }


    private void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        
        startYScale = transform.localScale.y;
    }

    private void Update()
    {

        //Performs ground check by "shooting a raycast down"
        //what is raycast? what do the numbers '0.5f + 0.2f' represent?
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f, whatIsGround);
        
        MyInput();
        SpeedControl();
        StateHandler();

        //Handle Drag
        if(grounded)
        
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //when to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //start crouch
        if(Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        //stop crouch
        if(Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        //Mode - crouching
        if(Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        
        

        //Mode - walking
        else if(grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        
        //Mode - sprinting
        if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        //Mode - Air
        else
        {
            state = MovementState.air;
        }
    }


    private void MovePlayer()
    {
        //calc movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //on slope
        if(OnSlope() && !exitingSlope)
        {
            rb.AddForce(20f * moveSpeed * GetSlopeMoveDirection(), ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force); 
            }
        }

        //on ground
        //What is ForceMode class?
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        //in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

            
        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        //Limiting speed on slope
        if(OnSlope() && !exitingSlope)
        {
            if(rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed; 
        }
        //limit speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if(flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        //reset y velocity to 0 to jjump at same height
        //what is 0f?
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        //rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private bool OnSlope()
    {
        //out slopeHit stores info of object we hit in the slopeHit variable
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
            
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        //Debug.Log("Debug log getSlopeMoveDirection"+Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized);
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}