using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEngine;

public class MoleHutBehavior : MonoBehaviour
{
    public Transform[] patrolPoints;
    public Transform m_Mole;
    public float speed = 2f;
    public float pauseTime = 1f;
    float minDelay = 1f;
    float maxDelay = 3f;
    int targetIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        m_Mole.transform.position = patrolPoints[1].transform.position;
        
        Invoke("MoveToNextWaypoint", Random.Range(minDelay, maxDelay));
    } 

    // Update is called once per frame
    void Update()
    {
        MoveToNextWaypoint();
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(pauseTime);
    }

    void MoveToNextWaypoint()
    {
        // Calculate the distance between the mole and its current target waypoint.
        float distanceToTarget = Vector3.Distance(m_Mole.transform.position, patrolPoints[targetIndex].position);

        // If the mole is close enough to its target waypoint, move to the next waypoint.
        if (distanceToTarget < 0.1f)
        {

            targetIndex = (targetIndex + 1) % patrolPoints.Length;
            Timer();

        }

        // Calculate the direction to the target waypoint.
        Vector3 directionToTarget = (patrolPoints[targetIndex].transform.position - m_Mole.transform.position).normalized;

        // Move the mole in the direction of the target waypoint.
        m_Mole.transform.position += directionToTarget * speed * Time.deltaTime;

        // Rotate the mole towards the direction of the target waypoint.
        m_Mole.transform.rotation = Quaternion.LookRotation(directionToTarget);

    }
}
