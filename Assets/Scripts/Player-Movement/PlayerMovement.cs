using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerMovement : MonoBehaviour
{
    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;
       
    [Header("Slope Handling")]
    private RaycastHit slopeHit;
    private bool exitingSlope;
    public float playerHeight;

    [Header("Q3 movement")]
    public LayerMask whatIsGround;
    public float friction;
    public float ground_accelerate;
    public float max_velocity_ground;
    public float air_accelerate;
    public float max_velocity_air;
    public float airControlForce;
    

    [Header("UI Manager")]
    private UIManager uiManager;

    [Header("Gravity 2.0")]
    private bool grounded; // Handy to have for jumping, and we'll use it here too
    Vector3 currentGravity; // Just holds some data for us...
    ContactPoint[] cPoints; // ContactPoints are generated by Collision, and they hold lots of fun data.
    Vector3 groundNormal; //The angle that will be perpendicular to the point of contact that our Rigidbody will be grounded on.
    public float maxSlopeAngle; //The steepest you want the character to be able to stand firmly on. Steeper than this and they'll slide.
    Rigidbody rb;
    public bool canJumpCast;
    //Note: The loss in momentum when bhopping may be due from the rigid body
    // making contact with the ground. Perhaps using a normal collider 
    // and placing it slightly below the capsule will help smoothen this out.


    public bool sliding;

    private void Start()
    {
        rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        //legs = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider>();
        rb.useGravity = false; //we'll make our own!
        /*Freezing rotation is not necessary, but highly recommended
        if we're making a character rather than just some object
        that happens to be on a slope.*/
        rb.freezeRotation = true;


        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        readyToJump = true;
        
        startYScale = transform.localScale.y;
    }

    void FixedUpdate()
    {
        canJumpCast = Physics.CheckSphere(transform.position - new Vector3(0,playerHeight/2,0) , 0.2f, whatIsGround);
        //Debug.Log(canJumpCast);
        
        uiManager.UpdateVelocityTxt(rb.velocity.magnitude.ToString("F3"));
    }

    void OnCollisionStay(Collision ourCollision)
    {
        //Debug.Log(ourCollision);
        grounded = CheckGrounded(ourCollision);
    }

    void OnCollisionExit(Collision ourCollision)
    {
        /*It's okay to not have to check whether or not
        the Collision we're exiting is one we're grounded on,
        because it'll be reaffirmed next time OnCollisionStay runs.*/
        grounded = false;
        groundNormal = new Vector3(); //Probably not necessary, but a good habit, in my opinion
    }
    bool CheckGrounded(Collision newCol)
    {
        cPoints = new ContactPoint[newCol.contactCount];
        newCol.GetContacts(cPoints);
        foreach(ContactPoint cP in cPoints)
        {
            
            /*If the difference in angle between the direction of gravity
            (usually, downward) and the current surface contacted is
            less than our chosen maximum angle, we've found an
            acceptable place to be grounded.*/
            if(maxSlopeAngle > Vector3.Angle(cP.normal, -Physics.gravity.normalized))
            {
                groundNormal = cP.normal;
                return true;
            }
        }

        return false;
    }

    public void ObeyGravity()
    {
        if(grounded == false)
        {
            //normal gravity, active when not grounded.
            currentGravity = Physics.gravity;
        }
        else if(grounded == true)
        {
            /*Not normal gravity. Instead of going down, we go in the
            direction perpendicular to the angle of where we're standing. 
            This means whatever surface we're grounded on will be 
            effectively the same as standing on a perfectly horizontal 
            surface. Ergo, no sliding will occur. */
            Debug.Log(currentGravity);
            Debug.Log(-groundNormal);
            Debug.Log(Physics.gravity.magnitude);
            Debug.Log(-groundNormal * Physics.gravity.magnitude);

            currentGravity = -groundNormal * Physics.gravity.magnitude;
        }
        rb.AddForce(currentGravity, ForceMode.Acceleration);
    }

    // accelDir: normalized, at the start, direction that the player has requested to move (taking into account the movement keys and look direction)
    // prevVelocity: The current velocity of the player, before any additional calculations
    // accelerate: The server-defined player acceleration value
    // max_velocity: The server-defined maximum player velocity (this is not strictly adhered to due to strafejumping)
    public Vector3 MoveGround(Vector3 accelDir, Vector3 prevVelocity)
    {
        accelDir = accelDir.normalized;
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
        //Debug.Log(prevVelocity);
        float projVel = Vector3.Dot(prevVelocity, accelDir); // Vector projection of Current velocity onto accelDir.
        float accelVel = accelerate * Time.fixedDeltaTime; // Accelerated velocity in direction of movment
        //Debug.Log(accelVel);
        // If necessary, truncate the accelerated velocity so the vector projection does not exceed max_velocity
        //Debug.Log(projVel + accelVel);
        if(projVel + accelVel > max_velocity)
        {
            accelVel = max_velocity - projVel;
            //accelVel = accelVel * //Mathf.FloorToInt(accelVel);
        }
            //accelVel = max_velocity - projVel;

        
        return prevVelocity + accelDir * accelVel;
    }


    //accelDir Normalized at the end
    public void MoveAir(Vector3 accelDir, Vector3 prevVelocity)
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

    public void Crouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
    }

    public void Stand()
    {
        rb.AddForce(groundNormal * Physics.gravity.magnitude*2, ForceMode.Force);
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        //stand up calculations
        

    }
    public void Jump()
    {
        exitingSlope = true;
        //reset y velocity to 0 to jjump at same height
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }

    public void ResetJump()
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

    public bool GetGrounded()
    {
        return grounded;
    }

    public bool GetReadyToJump()
    {
        return readyToJump;
    }
    public float GetJumpCoolDown()
    {
        return jumpCooldown;
    }

    public void SetReadyToJump(bool i)//i = input
    {
        readyToJump = i;
    }

    public bool GetExitingSlope()
    {
        return exitingSlope;
    }

    public float GetStartYScale()
    {
        return startYScale;
    }
}
