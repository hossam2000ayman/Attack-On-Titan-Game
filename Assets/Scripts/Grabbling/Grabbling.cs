using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbling : MonoBehaviour
{
    [Header("References")]
    private PlayerMovement pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;


    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    
    public float overshootYAxis; //and do some calculation
    //because the force calculation function needs to know what is the highest point of the flight path should it be 
    //because obviously makes difference when rotate this path (curve) or this path (more and more curved)
    //so first let's calculate the lowest point of the player then let's get difference of y axis 
    //between player and the grapple point 
    //and then let's add the overshoot y-axis on top of this 
    //and of it's below the player we can just use the overshoot y-axis without any other calculation


    private Vector3 grapplePoint;


    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;


    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;


    private void Start()
    {
        pm = GetComponent<PlayerMovement>();

    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey))
        {
            StartGrapple();
        }

        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
        }
    }


    private void StartGrapple()
    {
        if (grapplingCdTimer > 0)
        {
            return;
            //mean by return the function
        }

        // deactivate active Swinging
        GetComponent<Swinging>().StopSwing();

        grappling = true;

        pm.freeze = true;

        RaycastHit hit;
        if(Physics.Raycast(cam.position , cam.forward , out hit, maxGrappleDistance , whatIsGrappleable))
        {
            //if you hit something just store the grapple point
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple) , grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);

        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0)
        {
            highestPointOnArc = overshootYAxis;
        }

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple),1f);

    }

    public void StopGrapple() //it's need to be public for calling in other scripts 
    {
        pm.freeze = false;


        grappling = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }

}
