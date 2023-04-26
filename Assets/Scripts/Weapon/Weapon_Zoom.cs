using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon_Zoom : MonoBehaviour
{
    [SerializeField] Camera fpsCamera;

    [SerializeField] float zoomedOutFOV = 90f;
    [SerializeField] float zoomedInFOV = 50f;



    

    bool zoomedInToggle = false;


    //kont bawageh moshkelah lma b3ml zoom we 23ml switch lel weapon kan msh manteky we yb2a fyh weapon msh zaher mn scoope
     void OnDisable()
    {
        //when you leave this weapon no matter what get the zoom out of there
        ZoomOut();
    }

    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (zoomedInToggle == false)
            {
                ZoomIn();
            }
            else
            {
                ZoomOut();
            }
        }
    }

    

    private void ZoomIn()
    {
        zoomedInToggle = true;
        fpsCamera.fieldOfView = zoomedInFOV;
    }

    private void ZoomOut()
    {
        zoomedInToggle = false;
        fpsCamera.fieldOfView = zoomedOutFOV;
    }
}
