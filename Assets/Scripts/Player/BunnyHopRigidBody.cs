using UnityEngine;

public class BunnyHopRigidbody : MonoBehaviour
{
    public float jumpForce = 5f;
    public float moveSpeed = 5f;
    public float maxVelocityChange = 10f;
    public float groundFriction = 5f;
    public LayerMask groundLayer;
    public Transform groundCheckTransform;
    public float groundCheckRadius = 0.1f;
    public float customGravity = 20f;
    public float slopeForce = 10f;

    //public KeyCode jumpKey = KeyCode.Space;

    private Rigidbody rb;
    private bool isGrounded;
    private bool onSlope;
    public Vector3 slopeNormal;
    public Vector3 moveDirection;

    public Vector3 velocity;
    public Vector3 externalForces;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        //rb.useGravity = false;
    }

    /*
    private void Update()
    {
        // Perform ground check
        isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundCheckRadius, groundLayer);

        // Check for player jump input
        if (isGrounded && Input.GetKeyDown(jumpKey))
        {
            Jump();
        }

        // Calculate move direction based on player input
        moveDirection = (transform.forward * Input.GetAxis("Vertical") +
                         transform.right * Input.GetAxis("Horizontal")).normalized;
    }

    
    private void FixedUpdate()
    {
        MovePlayer();
    }
    
    
    public void MovePlayer()
    {
        // Apply custom gravity if not grounded
        if (!isGrounded)
        {
            ApplyCustomGravity();
            AirStrafe(moveDirection, moveSpeed);
        }
        else
        {
            onSlope = CheckSlope(out slopeNormal);

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
    }
    */

   public void SimulatePhysics()
    {
        // Update velocity with external forces
        Vector3 externalForces = Vector3.zero; // Calculate any external forces here
        velocity += externalForces * Time.fixedDeltaTime;

        // Apply custom gravity if not grounded
        if (!isGrounded)
        {
            ApplyCustomGravity();
        }

        // Apply velocity change
        Vector3 newPosition = rb.position + velocity * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    public void ApplyCustomGravity()
    {
        // Apply custom gravity to velocity
        velocity += Vector3.down * customGravity * Time.fixedDeltaTime;
    }

    public bool CheckSlope(out Vector3 normal)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, groundCheckRadius + 0.1f, groundLayer))
        {
            normal = hit.normal;
            return Vector3.Angle(hit.normal, Vector3.up) > 0.1f;
        }
        else
        {
            normal = Vector3.up;
            return false;
        }
    }

    public void ApplySlopeGravity(Vector3 slopeNormal)
    {
        Vector3 slopeGravity = Vector3.ProjectOnPlane(Physics.gravity, slopeNormal);
        velocity += slopeGravity * Time.fixedDeltaTime;
    }

    public void MoveOnGround(Vector3 direction, float speed, float friction)
    {
        // Apply friction to slow down the player
        Vector3 frictionForce = -rb.velocity.normalized * friction * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + frictionForce);

        Acceleration(direction, speed);
    }

    public void Acceleration(Vector3 direction, float speed)
    {
        // Calculate target velocity based on move direction and speed
        Vector3 targetVelocity = direction * speed;

        // Calculate velocity change
        Vector3 velocityChange = targetVelocity - rb.velocity;
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        // Apply velocity change using rb.MovePosition
        rb.MovePosition(rb.position + velocityChange * Time.fixedDeltaTime);
    }

    public void AirStrafe(Vector3 direction, float speed)
    {
        Acceleration(direction, speed); // No friction in the air
    }

    public void Jump()
    {
        // Apply jump force
        velocity += transform.up * jumpForce;
    }

    public bool GetIsGrounded()
    {
        return isGrounded;
    }

    public bool GetOnSlope()
    {
        return onSlope;
    }
    public void SetOnSlope()
    {
        onSlope = CheckSlope(out slopeNormal);;
    }
}
