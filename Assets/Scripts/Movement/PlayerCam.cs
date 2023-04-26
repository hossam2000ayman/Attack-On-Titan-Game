using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; 
//for new DOT TWEEN ASSET from assets store (added in project)

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;
    public Transform camHolder;

    //private
    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // get mouse input
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;

        //human cannot rotate his face 180 degree 
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotate cam and orientation 
        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }


    //Camera Effects FOV ==> field of view
        public void DoFov(float endValue)
    {                            //imported
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }
    
    //in case of wall running
    public void DoTile(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }
}