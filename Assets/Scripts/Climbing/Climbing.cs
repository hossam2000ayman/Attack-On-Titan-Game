using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public PlayerMovement pm;
    public LedgeGrabbing lg;
    public LayerMask whatIsWall;

    [Header("Climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    private float climbTimer;

    public bool climbing;

    [Header("Climb Jumping")]

    public float climbJumpUpForce;
    public float climbJumpBackForce;

    public KeyCode jumpKey = KeyCode.Space;
    public int climbJumps;
    private int climbJumpsLeft;



    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;


    private Transform lastWall;
    private Vector3 lastWallNormal;
    public float minWallNormalAngleChange;

    [Header("Exiting")]
    public bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;


    private void Start()
    {
        lg = GetComponent<LedgeGrabbing>();
    }
    private void Update()
    {
        WallCheck();
        StateMachine();
        if (climbing && !exitingWall) {
            ClimbingMovement();
        }

        if (climbing)
        {
            ClimbingMovement();
        }
    }

    private void StateMachine()
    {
        //State 0 -  Ledge Grabbing
        if (lg.holding)
        {
            if (climbing) { 
                StopClimbing();
            }
            //everthing else gets handled by the SubStateMachine() in the ledge grabbing script
        }
        // State 1 - Climbing
        else if(wallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle && !exitingWall)
        {
            if(!climbing && climbTimer > 0){
                StartClimbing();
            }

            //timer
            //timer will countdown during climbing
            if (climbTimer > 0) 
            {
                climbTimer -= Time.deltaTime;
            }
            if(climbTimer < 0)
            {
                StopClimbing();
            }
        }

        //(see first State 3 then ...) State 2 - Exiting
        else if (exitingWall)
        {
            if (climbing)
            {
                StopClimbing();
            }
            if(exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }

            if (exitWallTimer < 0) {
                exitingWall = false;
            }
        }

        // State 3 - None (Not in state of climbing)
        else
        {
            if (climbing)
            {
                StopClimbing();
            }
        }
        if(wallFront && Input.GetKeyDown(jumpKey) && climbJumpsLeft > 0) 
        {
            ClimbJump();
        }
    }
    private void WallCheck()
    {                       //it's look like RayCastHit but not with point or plane it's with sphere or cylinder
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);


        //now when did you hit a new wall ?
        //there is 2 cases 
        //if the 2 objects have completely different Transform from the last one
        // or same object but different in normal and we compare between the  angle at the first and the last normal

        bool newWall = frontWallHit.transform != lastWall || Mathf.Abs(Vector3.Angle(lastWallNormal,frontWallHit.normal)) > minWallNormalAngleChange;


        if ((wallFront && newWall) ||  pm.grounded)
        {
            climbTimer = maxClimbTime;
            climbJumpsLeft = climbJumps; //reset the climbJumpsLeft
        }
    }

    private void StartClimbing()
    {
        climbing = true;
        pm.climbing = true;
        //you can change the camera "fov" when climbing (optional)


        //next we need to check whether or not we hit a wall (because in that case we want to reset ClimbJumps)
        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;
    }

    private void ClimbingMovement()
    {
        //I'm not using rb.AddForce because in climbing it's make smoothly (better)
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
    }

    private void StopClimbing()
    {
        climbing = false;
        pm.climbing = false;
    }

    private void ClimbJump()
    {
        if (pm.grounded) return;
                            //by default false
        if (lg.holding || lg.exitingLedge) return;
        exitingWall = true;
        exitWallTimer = exitWallTime; 
        Vector3 forceToApply = transform.up * climbJumpUpForce + frontWallHit.normal * climbJumpBackForce;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        climbJumpsLeft--;
    }
}
