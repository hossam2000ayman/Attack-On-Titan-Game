using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed; 
    public float walkSpeed;
    public float sprintSpeed;
    public float swingSpeed;
    public float dashSpeed;
    public float dashSpeedChangeFactor;
    public float maxYSpeed; //because i limit for the x and z axis in speed control need to limit the y axis during dashing
                            //(leh msh 3mltaha gowah speed control l2naha hat2asar fy ba2y el script movement l2n tabe3y ana bt7rk x , z axis )
    public float slideSpeed;
    public float wallrunSpeed;
    public float climbSpeed;
    public float airMinSpeed;


    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;//using in coroutine
    public float slopeIncreaseMultiplier;//

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
    public bool grounded;

    [Header("Slope  Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;
    [Header("References")]
    public Climbing climbingScript;

    [Header("Camera Effects")]
    public PlayerCam cam;
    public float grappleFov = 95f;


    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        freeze,//state.freeze
        unlimited,
        swinging,
        walking,
        sprinting,
        wallrunning,
        climbing,
        crouching, //to slow down when crouching
        dashing,
        sliding,
        air

    }

    public bool dashing;
    public bool sliding;
    public bool swinging;
    public bool crouching;
    public bool wallrunning;
    public bool climbing;
    public bool freeze;
    public bool activeGrapple;
    public bool unlimited;
    public bool restricted; //not able to move

     void Start()
    {
        //initialization
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        startYScale = transform.localScale.y; //save that the normal Y scale of the player
    }

     void Update()
    {
        // ground check                 //player        //ground                                 //layermask
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();
        //it's also to make sure that all groundDrag is turned off while our grappling (in grappling player)
        //otherwise you are not going to land exactly where you want to.
        // handle drag(in movement player)
        if (grounded && !activeGrapple || state == MovementState.walking || state == MovementState.sprinting || state == MovementState.crouching)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;

        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); //a,d
        verticalInput = Input.GetAxisRaw("Vertical"); //w , s

        // when to jump             //true            //true
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;//on air

            Jump();

            //                          //2 second
            Invoke(nameof(ResetJump), jumpCooldown); //to be able ot jump again
        }

        //start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale= new Vector3(transform.localScale.x , crouchYScale , transform.localScale.z);
            rb.AddForce(Vector3.down * 5f,ForceMode.Impulse);
        }
        //stop crouch
        if(Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);

        }
    }

    private bool keepMomentum;
    //private float desiredMoveSpeed;
    //private float lastDesiredMoveSpeed;
    private MovementState lastState;

    private void StateHandler()
    {

        if (swinging)
        {
            state = MovementState.swinging;
            desiredMoveSpeed = swingSpeed;
        }
        //Mode - Dashing

        else if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor; //multiplier
        }

        //Mode - Freeze
        else if (freeze)
        {
            state = MovementState.freeze;
            rb.velocity = Vector3.zero;
            desiredMoveSpeed = 0; //don't use moveSpeed = x; for now
        }
        //Mode - unlimited
        else if (unlimited)
        {
            state = MovementState.unlimited;
             
            return; //it's mean by "return the funciton"
        }
        //Mode - Climbing
        else if (climbing)
        {
            state = MovementState.climbing;
            desiredMoveSpeed = climbSpeed;
        }
        //Mode - WallRunning
        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed; //moveSpeed --> wallrunSpeed

        }

        //Mode - Sliding
        else if (sliding)
        {
            state = MovementState.sliding;
            if (OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
                keepMomentum = true; //must make sure about that 
            }
            else
            {
                desiredMoveSpeed = sprintSpeed;
            }
        }
        //make desiredMoveSpeed instead to moveSpeed to handle our momentum speed

        //Mode - Crouching
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        // Mode - Sprinting
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        // Mode - Air
        else
        {
            state = MovementState.air;
            if (moveSpeed < airMinSpeed)
            {
                desiredMoveSpeed = airMinSpeed;
            }

            if(desiredMoveSpeed < sprintSpeed)
            {
                desiredMoveSpeed = walkSpeed; //speedControl
            }
            else
            {
                desiredMoveSpeed = sprintSpeed;
            }
            lastDesiredMoveSpeed= desiredMoveSpeed; //final result
            lastState= state;
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;

        if (lastState == MovementState.dashing) {
            keepMomentum = true;
        }

        //check if desiredMoveSpeed has changed drastically
        /* if(Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0) //old coding
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed= desiredMoveSpeed;
        }
         lastDesiredMoveSpeed= desiredMoveSpeed;
    }*/
        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                //any other action(method) called Coroutines
                StopAllCoroutines(); //more efficient
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed; //normal
            }
        }
        lastDesiredMoveSpeed = desiredMoveSpeed;//final result
        lastState= state;

        //deactivate keepMomentum
        if (Mathf.Abs(desiredMoveSpeed - moveSpeed) < 0.1f) {
            keepMomentum = false;
        }
    }

    //change moveSpeed --> desiredMoveSpeed (over time)
    private float speedChangeFactor;
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        //smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;//default

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease * boostFactor;
            }
            else
            {
                time += Time.deltaTime * speedIncreaseMultiplier * boostFactor;
            }


            //time += Time.deltaTime;
            yield return null;
            //steeper slope --> more accelleration
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum= false;
    }

    private void MovePlayer()
    {


        //2- this is important otherwise you move while you grappling and missing your target
        if (activeGrapple)
        {
            return;
        }
        if (swinging)
        {
            return;
        }
        if (restricted)
        {
            return; // mean that the player can't move by using his key 
        }
        if (climbingScript.exitingWall)
        {
            return;
        }

        if(state == MovementState.dashing)
        {
            return;
        }

        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //on Slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);
            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f , ForceMode.Force);
            }
        }

        // on ground
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        // in air
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
        //turn gravity off while on slope

        if (!wallrunning) {
            rb.useGravity = !OnSlope(); //importants
        } 

    }

    private void SpeedControl()
    {
        //1- deactivate speed control while grappling
        if (activeGrapple)
        {
            return;
        }
        //limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed; //uniform speed
            }
        }

        //limiting speed on ground or in air 
        else
        {
                                                //reset Y Velocity
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }                               //a,d        //constant (jump)                  //w,s
        }

        //limit y velocity
        if(maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
        {
            //limited with maxYSpeed
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                                                //force is occur """once"""
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope= false;
    }

    private bool enableMovementOnNextTouch;


    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;
        
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);//equaitons

        Invoke(nameof(SetVelocity), 0.1f); //now the velocity getsapplied after 0.1 seconds
        
        //after using the grappling for more than 2 seconds probably something went wrong so let's allow the movement again 
        Invoke(nameof(ResetRestrictions), 2f);
    }

    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;//current velocity stop

        cam.DoFov(grappleFov);

    }

    public void ResetRestrictions()
    {
        activeGrapple= false;
        cam.DoFov(85f);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch= false;
            ResetRestrictions();

            GetComponent<Grabbling>().StopGrapple();
        }
    }


    //make it public to be shared with other script
    //slope check
    public bool OnSlope()
    {                         //PLAYER             //force               //store the info of objects that we hit in slope hit variable
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0; //true or false
        }
        
        //otherwise
        return false;
    }


    //to be shared with other script
    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized; //we projected our normal direction on 
    }

    //Kinematic Equations (E03: Ball problem) explain physical principle behind calculation
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y; 
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x , 0f , endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    
}