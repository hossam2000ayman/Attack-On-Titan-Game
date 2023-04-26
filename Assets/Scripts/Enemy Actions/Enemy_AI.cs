using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//for using NavMeshAgent

public class Enemy_AI : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] Transform target;
    [SerializeField] float chaseRange = 5f;
    [SerializeField] float turnSpeed = 5f;

    NavMeshAgent navMeshAgent;

    float distanceToTarget = Mathf.Infinity;
    bool isProvoked = false;

    
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {                                       //me(player)         //enemy
        distanceToTarget = Vector3.Distance(target.position,transform.position);

        if (isProvoked)
        {
            EngageTarget();
        }else if(distanceToTarget <= chaseRange)
        {
            isProvoked= true;

        }
    }

    //if enemy take a damage you must be trigger or begin attack on player
    public void OnDamageTaken()
    {
        isProvoked = true;
    }

    private void EngageTarget()
    {
        FaceTarget();
                                //the place the enemy stopping near to player for attack on player
        if (distanceToTarget >= navMeshAgent.stoppingDistance)
        {
            ChaseTarget();
        }
                                
        if(distanceToTarget <= navMeshAgent.stoppingDistance)
        {
            AttackTarget();
        }

    }

    private void ChaseTarget()
    {
        GetComponent<Animator>().SetBool("attack", false);
        //GetComponent<Animator>().SetTrigger("move"); 

        //go to the target          //player position (me)
        navMeshAgent.SetDestination(target.position);
    }

    private void AttackTarget()
    {
        GetComponent<Animator>().SetBool("attack",true);
        //hit the player
    }

    private void FaceTarget()
    {
                            //where is target - our position
        Vector3 direction = (target.position - transform.position).normalized;
        
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x,0,direction.z));
                        //allow us to turn smoothly between 2 vectors
        transform.rotation = Quaternion.Slerp(transform.rotation,lookRotation,Time.deltaTime * turnSpeed);

    }

     void OnDrawGizmosSelected()
    {
        //Display the explosion radius when selected
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position , chaseRange);
    }

}
