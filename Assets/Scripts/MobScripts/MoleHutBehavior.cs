using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEngine;

public class MoleHutBehavior : MonoBehaviour
{
    public Transform[] patrolPoints;
    public Transform m_Mole;
    public Animator animator;
    public float speed = 2f;
    public float pauseTime = 1f;
    float minDelay = 0.5f;
    float maxDelay = 2.5f;
    int targetIndex = 0;
    bool notRotate = false;

    // Start is called before the first frame update
    void Start()
    {
        m_Mole.transform.position = patrolPoints[1].transform.position;
        animator = m_Mole.GetComponent<Animator>();
    } 

    // Update is called once per frame
    void Update()
    {

        if(pauseTime > 0f)
        {
            pauseTime -= Time.deltaTime;
        }
        else
        {
            notRotate = false;
            MoveToNextWaypoint();
        }
        

    }

    void MoveToNextWaypoint()
    {
        // Calculate the distance between the mole and its current target waypoint.
        float distanceToTarget = Vector3.Distance(m_Mole.transform.position, patrolPoints[targetIndex].position);

        // If the mole is close enough to its target waypoint, move to the next waypoint.
        if (distanceToTarget < 0.1f)
        {

            targetIndex = (targetIndex + 1) % patrolPoints.Length;
            pauseTime = Random.Range(minDelay, maxDelay);
            notRotate = true;
        }

        if(!notRotate)
        {
            // Calculate the direction to the target waypoint.
            Vector3 directionToTarget = (patrolPoints[targetIndex].transform.position - m_Mole.transform.position).normalized;

            // Move the mole in the direction of the target waypoint.
            m_Mole.transform.position += directionToTarget * speed * Time.deltaTime;

            // Rotate the mole towards the direction of the target waypoint.
            m_Mole.transform.rotation = Quaternion.LookRotation(directionToTarget);
        }

    }
}
