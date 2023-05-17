using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoleController : MonoBehaviour
{
    //Waypoint array
    public Transform[] points;

    //Essentially self
    private NavMeshAgent agent;

    //Next point to move towards/if you are there
    private int destPoint = 0;
    bool atPoint = true;

    //Determine if player is close
    bool playerInRange = false;

    //Range that spherecast looks for player
    public float sightRange = 8f;

    //Player's transform
    Transform player;

    //Layermask for Spherecast 
    LayerMask isPlayer;


    void Start()
    {
       //Initialize agent, player, playerMask, start patrolling after random 0-3 seconds
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
        isPlayer = LayerMask.GetMask("Player");
        
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
        if(distanceToWalkPoint.magnitude < 0.1f)
        {
            atPoint = true;
        }

    }

    void Chasing()
    {
        //Speed up a bit and move towards player
        agent.speed = 3f;
        agent.destination = player.transform.position;
    }

}
