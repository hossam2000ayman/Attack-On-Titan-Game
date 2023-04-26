using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Dashing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerCam;
    private Rigidbody rb;
    private PlayerMovement pm;


    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;
    public float maxDashYSpeed;

    [Header("Camera Effects")]
    public PlayerCam cam;
    public float dashFov;


    [Header("Settings")]
    public bool useCameraForward = true;
    public bool allowAllDirection = true;
    public bool disableGravity = false;
    public bool resetVel = true;


    [Header("Cooldown")]
    public float dashCd;
    private float dashCdTimer;

    [Header("Input")]
    public KeyCode dashKey = KeyCode.E;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();  
        pm= GetComponent<PlayerMovement>();

    }

    private void Update()
    {
        if (Input.GetKeyDown(dashKey)) {
            Dash();
        }

        if(dashCdTimer > 0)
        {
            dashCdTimer -= Time.deltaTime;
        }
    }

    private void Dash()
    {
        if(dashCdTimer > 0)
        {
            return; //Dash();
        }
        else
        {
            dashCdTimer = dashCd;
        }

        pm.dashing = true;
        pm.maxYSpeed = maxDashYSpeed;

        //camera effect when dasing
        cam.DoFov(dashFov);


        Transform forwardT;
        if (useCameraForward)
        {
            forwardT = playerCam;
        }
        else
        {
            forwardT = orientation;
        }

        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
        {
            rb.useGravity = false;
        }

        delayedForceToApply = forceToApply;

        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply;
    

    

    private void DelayedDashForce()
    {

        if (resetVel)
        {
            rb.velocity = Vector3.zero;
        }
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);

    }
    private void ResetDash()
    {
        pm.dashing = false;
        pm.maxYSpeed = 0;
        //reset camera effect
        cam.DoFov(85f);

        if (disableGravity)
        {
            rb.useGravity = true;
        }

    }

    private Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if (allowAllDirection)
        {
            //this will allow you to dash in 8 different direction depending on which key pressing
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        }
        else
        {
            direction = forwardT.forward;
        }
        if(verticalInput == 0 && horizontalInput == 0)
        {
            direction= forwardT.forward;
        }

        return direction.normalized;
    }

}
