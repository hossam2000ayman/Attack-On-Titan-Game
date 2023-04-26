using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Health : MonoBehaviour
{

    [SerializeField] float healthPoints = 100f;

    //create a public method which reduces hitpoints by the amount of damage 
    public void TakeDamage(float damage)
    {
        healthPoints -= damage;

        if (healthPoints <= 0)
        {
            //die
            GetComponent<Death_Handler>().HandleDeath();
        }
    }
}
