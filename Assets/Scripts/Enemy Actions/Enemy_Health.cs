using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{ 
    public float hitPoints = 300f; //enemy health
    [SerializeField]Animator animator;

    //create a public method which reduces hitpoints by the amount of damage 
    public void TakeDamage(float damage)
    {
        //GetComponent<Enemy_AI>().OnDamageTaken();

        //2a2oloh(enemy) meen 2ely darabak (make him provoke that some one hit him )
        BroadcastMessage("OnDamageTaken");
        hitPoints -= damage;

        if(hitPoints <= 0)
        {
            //die animation
            animator.SetTrigger("death");
            
            //die
            Invoke("Disappear",3f);
        }
    }

     void Disappear()
    {
        Destroy(gameObject);
    }
}
