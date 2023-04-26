using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class LedgeGrabbing : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement pm;
    public Transform orientation;
    public Transform cam;
    public Rigidbody rb;

    [Header("Ledge Grabbing")]
    public float moveToLedgeSpeed;
    public float maxLedgeGrabDistance;
    public float minTimeOnLedge;
    private float timeOnLedge;

    public bool holding;

    [Header("Ledge Jumping")]
    public KeyCode jumpKey = KeyCode.Space;
    public float ledgeJumpForwardForce;
    public float ledgeJumpUpwardForce;




    [Header("Ledge Detection")]
    public float ledgeDetectionLenght;
    public float ledgeSphereCastRadius;
    public LayerMask whatIsLedge;

    private Transform lastLedge;
    private Transform currLedge;

    private RaycastHit ledgeHit;

    [Header("Exiting")]
    public bool exitingLedge;
    public float exitLedgeTime;
    private float exitLedgeTimer;



    private void Update()
    {
        LedgeDetection();
        SubStateMachine();
    }
    //so the idea is that the player is some where here  and looking at the ledge then first he get's pull to the ledge and then he freez his rigidbody
    private void SubStateMachine()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        bool anyInputKeyPressed = horizontalInput != 0 || verticalInput != 0;

        //SubStateMachine 1 - Holding onto ledge
        if (holding)
        {
            FreezeRigidbodyOnLedge();
            timeOnLedge += Time.deltaTime;

            if (timeOnLedge > minTimeOnLedge && anyInputKeyPressed)
            {
                ExitLedgeHold();
            }
            if (Input.GetKeyDown(jumpKey))
            {
                LedgeJump();
            }
        }

        //Substate 2 - Exiting Ledge
        else if (exitingLedge)
        {
            if(exitLedgeTimer > 0)
            {
                exitLedgeTimer-= Time.deltaTime;
            }
            else
            {
                exitingLedge = false;
            }
        }

    }

    //and now let's implement the Ledgedeteciton for this we are going uses sphereCast
    private void LedgeDetection()
    {
        
        bool ledgeDetected = Physics.SphereCast(transform.position, ledgeSphereCastRadius, cam.forward, out ledgeHit, ledgeDetectionLenght, whatIsLedge);

        if (!ledgeDetected) { 
            return;
        }
        float distanceToLedge = Vector3.Distance(transform.position , ledgeHit.transform.position);

        if(ledgeHit.transform == lastLedge)
        {
            return;
        }
        if(distanceToLedge < maxLedgeGrabDistance && !holding)
        {
            EnterLedgeHold();
        }
    }
    private void LedgeJump()
    {
        ExitLedgeHold();

        Invoke(nameof(DelayedJumpForce),0.05f);

    }
    private void DelayedJumpForce()
    {
        Vector3 forceToAdd = cam.forward * ledgeJumpForwardForce + orientation.up * ledgeJumpUpwardForce;
        rb.velocity = Vector3.zero;
        rb.AddForce(forceToAdd, ForceMode.Impulse);
    }

    private void EnterLedgeHold()
    {
        holding = true;
        pm.unlimited = true;
        pm.restricted= true;

        currLedge = ledgeHit.transform;
        lastLedge = ledgeHit.transform;

        rb.useGravity= false;
        //remove all momentum of the rigidBody
        rb.velocity= Vector3.zero;
    }

    private void FreezeRigidbodyOnLedge()
    {
        rb.useGravity= false;
        Vector3 directionToLedge = currLedge.position - transform.position;
        float distanceToLedge = Vector3.Distance(transform.position, currLedge.position);

        //Move Player towards ledge (ENTER THE LEDGE)
        if(distanceToLedge > 1f)
        {
            if (rb.velocity.magnitude < moveToLedgeSpeed)
            {
                rb.AddForce(1000f * moveToLedgeSpeed * Time.deltaTime * directionToLedge.normalized);
            }
        }

        //Hold onto ledge (STAY)
        else
        {
            if (!pm.freeze) {
                pm.freeze = true;
            }
            if (pm.unlimited)
            {
                pm.unlimited = false;
            }
        }
        if(distanceToLedge > maxLedgeGrabDistance)
        {
            ExitLedgeHold();
        }
    }

    private void ExitLedgeHold()
    {
        exitingLedge = true;
        exitLedgeTimer = exitLedgeTime;
        holding = false;
        timeOnLedge = 0f;

        pm.restricted = false;
        pm.freeze = false;
        rb.useGravity = true;

        StopAllCoroutines(); //stop all the previous method executed
        Invoke(nameof(ResetLastLedge), 1f);
    }
    private void ResetLastLedge()
    {
        lastLedge = null;
    }

}
