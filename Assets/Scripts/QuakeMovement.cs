using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class QuakeMovement : MonoBehaviour
{

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
    public KeyCode crouchKey = KeyCode.C;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;    

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Q3 movement")]
    public float friction;
    public float ground_accelerate;
    public float max_velocity_ground;
    public float air_accelerate;
    public float max_velocity_air;
    public float airControlForce;

    [Header("UI Manager")]
    private UIManager uiManager;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState 
    {
        walking,
        sprinting,
        crouching,
        sliding,
        air
    }
    public bool sliding;

    private void Start()
    {
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        
        startYScale = transform.localScale.y;
    }

    private void Update()
    {

        //Performs ground check by shooting a raycast down
        if(Input.GetKey(crouchKey))
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * crouchYScale * 0.5f + 0.2f, whatIsGround);
        else    
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        
        MyInput();
        StateHandler();

        uiManager.UpdateVelocityTxt(rb.velocity.magnitude.ToString("F3"));
    }

    private void FixedUpdate()
    {
        MovePlayer();

    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); //a+d
        verticalInput = Input.GetAxisRaw("Vertical"); //w+s

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
        }
        //Mode - walking
        else if(grounded)
        {
            state = MovementState.walking;
        }
        //Mode - sprinting
        if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
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
            //Debug.Log("SLOPE DETECTED!");
            
            rb.velocity = MoveGround(GetSlopeMoveDirection(moveDirection), rb.velocity);
            rb.drag = 2;
            //creates consistent player velocity on slope
            //TODO: This currently kills momentum on some bhop surfaces, fix this
            //NOTE: When walking on the ground, velocity will get close to 7 but not reach (6.8)
            //however when on slope, this sets the velocity straight to 7. Investigate why its not reaching closer to 7 in the first palce
            
                
            
            //walking up/down
        }

        //walking
        else if(grounded)
        {
            rb.velocity = MoveGround(moveDirection.normalized, rb.velocity);
            rb.drag = 0;
        }
            //rb.velocity = MoveGround(moveDirection.normalized, rb.velocity);

        //in air
        else if(!grounded)
            MoveAir(moveDirection, rb.velocity);

            
        // turn gravity off while on slope
        
        rb.useGravity = !OnSlope();
    }

    // accelDir: normalized direction that the player has requested to move (taking into account the movement keys and look direction)
    // prevVelocity: The current velocity of the player, before any additional calculations
    // accelerate: The server-defined player acceleration value
    // max_velocity: The server-defined maximum player velocity (this is not strictly adhered to due to strafejumping)
    private Vector3 MoveGround(Vector3 accelDir, Vector3 prevVelocity)
    {
        // Apply Friction
        float speed = prevVelocity.magnitude;
        if (speed != 0) // To avoid divide by zero errors
        {
            //Debug.Log("Friction!!");
            float drop = speed * friction * Time.fixedDeltaTime;
            prevVelocity *= Mathf.Max(speed - drop, 0) / speed; // Scale the velocity based on friction.
        }

        // ground_accelerate and max_velocity_ground are server-defined movement variables
        return Accelerate(accelDir, prevVelocity, ground_accelerate, max_velocity_ground);
    }

    //TODO: Unoptimized Acceleration calculations, currently doing it seperately: for ground and air. 
    // Unite these for better readability/performance
    private Vector3 Accelerate(Vector3 accelDir, Vector3 prevVelocity, float accelerate, float max_velocity)
    {
        
        float projVel = Vector3.Dot(prevVelocity, accelDir); // Vector projection of Current velocity onto accelDir.
        float accelVel = accelerate * Time.fixedDeltaTime; // Accelerated velocity in direction of movment
        
        // If necessary, truncate the accelerated velocity so the vector projection does not exceed max_velocity
        //Debug.Log(projVel + accelVel);
        Debug.Log(projVel);
        if(projVel + accelVel > max_velocity)
        {
            accelVel = max_velocity - projVel;
            //accelVel = accelVel * //Mathf.FloorToInt(accelVel);
        }
            //accelVel = max_velocity - projVel;

        
        return prevVelocity + accelDir * accelVel;
    }

    private void MoveAir(Vector3 accelDir, Vector3 prevVelocity)
    {
        // project the velocity onto the movevector
        Vector3 projVel = Vector3.Project(prevVelocity, accelDir);

        // check if the movevector is moving towards or away from the projected velocity
        bool isAway = Vector3.Dot(accelDir, projVel) <= 0f;

        // only apply force if moving away from velocity or velocity is below MaxAirSpeed
        if (projVel.magnitude < max_velocity_air || isAway)
        {
            // calculate the ideal movement force
            Vector3 vc = accelDir.normalized * airControlForce;

            // cap it if it would accelerate beyond MaxAirSpeed directly.
            if (!isAway)
            {
                vc = Vector3.ClampMagnitude(vc, max_velocity_air - projVel.magnitude);
            }
            else
            {
                vc = Vector3.ClampMagnitude(vc, max_velocity_air + projVel.magnitude);
            }

            // Apply the force
            rb.AddForce(vc, ForceMode.VelocityChange);
        }
        
    }
    private void Jump()
    {
        exitingSlope = true;
        //reset y velocity to 0 to jjump at same height
        if(Input.GetKey(crouchKey))
            rb.velocity = new Vector3(rb.velocity.x, jumpForce * (crouchYScale + 0.15f), rb.velocity.z);
        else
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        //out slopeHit stores info of object we hit in the slopeHit variable
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
            
        }

        return false;
    }
    
    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}
