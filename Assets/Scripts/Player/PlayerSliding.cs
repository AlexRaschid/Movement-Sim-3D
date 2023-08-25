using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSliding : MonoBehaviour
{
    [Header("References")]
        public Transform orientation;
        public Transform playerObj;
        private Rigidbody rb;
        //private PlayerMain pm;
        private DynamicPlayerMovement qm;
        
    [Header("References")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;

    public float slideYScale;
    private float startYScale;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //pm = GetComponent<PlayerMain>();
        qm = GetComponent<DynamicPlayerMovement>();

        startYScale = playerObj.localScale.y;
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); //a+d
        verticalInput = Input.GetAxisRaw("Vertical"); //w+s
        
        if(Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput !=0))
        {
            StartSlide();
        }

        if(Input.GetKeyUp(slideKey) && qm.sliding)
        {
            StopSlide();
        }
    }

    void FixedUpdate()
    {
        
    }
    private void StartSlide()
    {
        qm.sliding = true;


        /*if(!Input.GetKey(pm.crouchKey))
        {
            transform.localScale = new Vector3(playerObj.localScale.x, qm.crouchYScale, playerObj.localScale.z);
            rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);  
        }*/
            

        slideTimer = maxSlideTime;
    }

    public void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //sliding normal - when player not on slope or movign upwards
        if(!qm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.fixedDeltaTime;
        }
        
        //sliding down a slope
        else 
        {
            rb.AddForce(qm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        if(slideTimer <= 0)
            StopSlide();
    }

    private void StopSlide()
    {
        qm.sliding = false;
        
        //playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
        transform.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }



}
