using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

    Player_Health target;

    [SerializeField] float damage = 40f;

 void Start()
    {
        //bind with target (target.GetComponent<Player_Health>().TakeDamage(damage))
        target = FindObjectOfType<Player_Health>();
    }

    

    public void AttackHitEvent()
    {
        //if the player not hit 
        if(target== null)
        {
            return;
        }
        //otherwise 
        target.TakeDamage(damage);
    }
}
