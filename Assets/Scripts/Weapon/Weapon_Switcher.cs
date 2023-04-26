using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Switcher : MonoBehaviour
{
    [SerializeField] int currentWeapon = 0; // carbine

     void Start()
    {
        SetWeaponActive();
    }

     void Update()
    {
        int previousWeapon = currentWeapon;

        ProcessKeyInput();
        ProcessScrollWheel();

        if(previousWeapon != currentWeapon)
        {
            SetWeaponActive();
        }
    }

     void ProcessScrollWheel()
    {
        if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if(currentWeapon >= transform.childCount - 1)
            {
                currentWeapon = 0;
            }
            else
            {
                currentWeapon++;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (currentWeapon <= 0)
            {
                currentWeapon = transform.childCount - 1;
            }
            else
            {
                currentWeapon--;
            }
        }
    }

     void ProcessKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeapon = 0;//carbine
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeapon = 1; //shotgun
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentWeapon = 2; //pistol
        }
    }

    private void SetWeaponActive()
    {
        //what is active and what is inactive
        int weaponIndex = 0;

        foreach(Transform weapon in transform)
        {
            
            if(weaponIndex == currentWeapon)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }

            weaponIndex++;
        }
    }
}
