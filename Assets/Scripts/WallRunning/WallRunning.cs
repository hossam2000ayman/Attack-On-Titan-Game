using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WallRunning : MonoBehaviour
{
    [Header("Wall running")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float wallClimbSpeed;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Input")]
    public KeyCode jumpKey = KeyCode.Space;

    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;
    private bool upwardsRunning;
    private bool downwardsRunning;
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exiting")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;
    [Header("Gravity")]
    public bool useGravity;
    public float gravityCounterForce;


    [Header("References")]
    public Transform orientation;
    public PlayerCam cam;
    private PlayerMovement pm;
    private LedgeGrabbing lg;
    private Rigidbody rb;
    


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        lg = GetComponent<LedgeGrabbing>();

    }
    ///now before the player  should start any wallrunning movement you of course need to check if there is a wall in range
    ///we should do this by shooting(point)(bnshawer) our raycast to the left and right

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.wallrunning)
        {
            WallRunningMovement();
        }
    }
    private void CheckForWall()
    {                               //start point       //direction         //store hit info    //distance      //layer
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);
    }

    //and there is one more check we need to perform 
    //he is checking if player is high enough to the air to start wallrunning

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down , minJumpHeight , whatIsGround);
    }

    private void StateMachine()
    {
        
        //Getting Inputs 
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        // State 1 - Wallrunning
        if((wallLeft ||  wallRight) && verticalInput > 0 &&  AboveGround() && !exitingWall)
        {
            //start wallrun here!
            if (!pm.wallrunning)
            {
                StartWallRun();
            }
             // wallrun timer
             if(wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;
            }
            if (wallRunTimer <= 0 && pm.wallrunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }
            {

            }


            //wall jump
            if (Input.GetKeyDown(jumpKey))
            {
                WallJump();
            }
            
        }
        //State 2 - Exiting
        else if (exitingWall) {
            if (pm.wallrunning)
            {
                StopWallRun();
            }

            if(exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }
            if(exitWallTimer <= 0)
            {
                exitingWall = false;

            }
        }

        //State 3 - None
        else
        {
            if (pm.wallrunning)
            {
                StopWallRun();
            }
        }
    }

    //before we continue you have to understand that i usually handle the all speed limitation in the playermovement script and then 
    //add forcesin separate scripts


    private void StartWallRun()
    {
        pm.wallrunning = true;
        wallRunTimer = maxWallRunTime;
        //we didn't want to call it every frame
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //apply camera effects
        cam.DoFov(90f);
        if (wallLeft)
        {
            cam.DoTile(-5f);
        }
        if (wallRight)
        {
            cam.DoTile(5f);
        }


    }

    //the hardest part here is to find forward key direction of the wall because it's has to work no matter how was wall rotated (in any case "y3ny") 
    //in this case we use Vector3.Cross(a,b) a funciton that takes right and up direction and then return wall-forward direction
    //(right direction is called ==> wallNormal (in just a direction pointing away from wall) and it's easy to get it ) because we already store RayCasthit information

    private void WallRunningMovement()
    {
        rb.useGravity = useGravity;

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude){
            wallForward = -wallForward;
        }
        //forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        //upwards/downwards force
        if (upwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        }

        if (downwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);
        }

        //push to wall force 
        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput <0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }

        //tp weaken gravity a little bit
        if (useGravity)
        {
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
        }
    }
    private void StopWallRun()
    {
        pm.wallrunning = false;

        //reset camera effects
        cam.DoFov(80f);
        cam.DoTile(0f);

    }

    private void WallJump()
    {
        if (lg.holding || lg.exitingLedge) { return; }
        //enter exiting wall state
        exitingWall = true;
        exitWallTimer = exitWallTime;
        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        //reset y velocity and then // add force 
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

    }
    

}
