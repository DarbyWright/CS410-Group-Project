using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class MoleController : MonoBehaviour
{
    //Waypoint array
    public Transform[] points;

    //Essentially self
    private NavMeshAgent agent;

    //Next point to move towards/if you are there
    public int destPoint = 0;
    bool atPoint = true;

    //Determine if player is close
    bool playerInRange = false;

    //Range that spherecast looks for player
    public float sightRange = 8f;

    //Player's transform
    Transform player;

    //Layermask for Spherecast 
    LayerMask isPlayer;

    public bool isMoving = false;
    public bool isRunning = false;
    public bool inRange = false;
    Animator animator;

    void Start()
    {
       //Initialize agent, player, playerMask, start patrolling after random 0-3 seconds
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
        isPlayer = LayerMask.GetMask("Player");
        animator = GetComponent<Animator>();

        
        //Start randomly to help create openings for hut jumps
        Invoke(nameof(Patrolling), Random.Range(0f, 3f));
    }

    void Update()
    {
        //Check if player is near
        playerInRange = Physics.CheckSphere(transform.position, sightRange, isPlayer);

        
        if(!playerInRange)
        {
            Patrolling();
        }
        else
        {
            Chasing();
        }
         
    }

    void Patrolling()
    {

        if (isRunning)
        {
            isRunning = false;
            inRange = false;
            animator.SetBool("inRange", false);
        }

        isMoving = true;
        animator.SetBool("isMoving", isMoving);
        if(atPoint)
        {
            // Set the agent to go to the currently selected destination.
            agent.destination = points[destPoint].position;

            // Choose the next point in the array as the destination,
            // cycling to the start if necessary.
            destPoint = (destPoint + 1) % points.Length;

            atPoint = false;
        }

        //Find distance to new point and if you are close enough set flag to find next point 
        Vector3 distanceToWalkPoint = transform.position - agent.destination;
        if(distanceToWalkPoint.magnitude < 2f)
        {
            atPoint = true;
        }

    }

    void Chasing()
    {
        isRunning = true;
        animator.SetBool("isRunning", isRunning);
        //Speed up a bit and move towards player
        agent.speed = 12f;

        if(Vector3.Distance(transform.position, agent.destination) < 6f)
        {
            animator.SetBool("inRange", true);
        }

        if(Vector3.Distance(transform.position, agent.destination) > 10f && inRange)
        {
            inRange = false;
            animator.SetBool("inRange", false);
        }
        
        Vector3 direction = (player.transform.position - transform.position);
        agent.destination = player.transform.position;
        
    }

}
