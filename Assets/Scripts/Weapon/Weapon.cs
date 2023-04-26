using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Camera FPCamera;
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 30f;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject hitEffect;
    [SerializeField] Ammo ammoSlot;
    [SerializeField] AmmoType ammoType;
    [SerializeField] float timeBetweenShots = 0.5f;

    //because pistol not equal shotgun not equal carbrine as it different in firerate
    bool canShoot = true;

    //when script is enabled on the object(i see it's not important)
    //ana 2slun fy 2awel kedah kedah badrab 3alatool fa msh me7tagah eh r2yoko??? 
    
    private void OnEnable()
    {
        canShoot = true;
    }

    private RaycastHit hit;


    void Update()
    {
        if (Input.GetKey(KeyCode.G) && canShoot == true)
        {
            StartCoroutine(Shoot());
        }
    }


    IEnumerator Shoot()
    {
        canShoot = false;
        //if you have ammo then fire otherwise hard luck no ammo
        if (ammoSlot.GetCurrentAmmo(ammoType) > 0)
        {
            PlayMuzzleFlash();
            ProcessRaycast();
            ammoSlot.ReduceCurrentAmmo(ammoType);
        }
        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
    }

    private void PlayMuzzleFlash()
    {
        muzzleFlash.Play();
    }

    private void ProcessRaycast()
    {
        if (Physics.Raycast(FPCamera.transform.position, FPCamera.transform.forward, out hit, range))
        {
            //Debug.Log("I hit this thing " + hit.transform.name);

            CreateHitImpact(hit);
            //TODO add some hit effect for visual players
            Enemy_Health target = hit.transform.GetComponent<Enemy_Health>();

            if (target == null)
            {
                return;
            }
            //call a method on enemy health that decrease enemy's health
            target.TakeDamage(damage);



        }//if i shoot in sky or nothing that raycast get it not return error 
        else
        {
            //no thing no references
            return;
        }
    }

    private void CreateHitImpact(RaycastHit hit)
    {
       GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impact, 0.1f);
    }
}