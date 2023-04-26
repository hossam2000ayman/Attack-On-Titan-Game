using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float health = 3;

    [Header("Combat")]
    [SerializeField] float attackCD = 3f;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float aggroRange = 4f;
    [SerializeField] float playerHight = 1f;

    GameObject player;
    NavMeshAgent agent;
    Animator animator;
    float timepassed;
    float newDestinationCD = 0.5f;

    //Patroling
    [Header("Patroling")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public LayerMask whatIsGround;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("speed", agent.velocity.magnitude / agent.speed);

        if(timepassed >= attackCD)
        {
            if(Vector3.Distance(player.transform.position, transform.position) <= attackRange)
            {
                if(player.transform.position.y >= playerHight)
                {
                    animator.SetTrigger("attack2");
                    timepassed = 0;
                }
                else
                {
                    animator.SetTrigger("attack");
                    timepassed = 0;

                }
                
            }

        }
        timepassed += Time.deltaTime;

        if(newDestinationCD <= 0 && Vector3.Distance(player.transform.position, transform.position) <= aggroRange)
        {
            
            transform.LookAt(player.transform);
            newDestinationCD = 0.5f;
            agent.SetDestination(player.transform.position);
            
        }
        newDestinationCD -= Time.deltaTime;

        if (newDestinationCD <= 0 && Vector3.Distance(player.transform.position, transform.position) > aggroRange)
        {
            newDestinationCD = 0.5f;
            Patroling();

        }
        newDestinationCD -= Time.deltaTime;
    }

    //walking in specific range (random)
    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpont reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random pont in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        //make sure the enemy does not walk out of map bound
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }


    void Die()
    {
        animator.SetTrigger("death");
        rb.constraints = RigidbodyConstraints.FreezePosition;
        Invoke("Disappear", 3f);
    }

    void Disappear()
    {
        Destroy(this.gameObject);
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        animator.SetTrigger("damage");

        if(health <= 0)
        {
            Die();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
