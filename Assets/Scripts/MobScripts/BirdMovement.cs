using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class BirdMovement : MonoBehaviour
{
    // Speed of the bird's movement in meters per second.
    public float speed = 6f;

    // Array of waypoints that define the bird's flight path.
    public Waypoint[] waypoints;

    // Current index of the bird's target waypoint.
    public int targetWaypointIndex = 0;

    // Starting Waypoint Object
    public GameObject startingWaypoint;

    // Rate of rotation of the bird in degrees per second.
    public float rotationSpeed = 10f;

    // Prefab of the rock object to drop.
    public GameObject rockPrefab;

    // Distance to drop the rock from the bird.
    public float dropDistance = 20f;
    public float rockDropTimer = 0f;
    public float rockDropInterval = 2.5f;

    bool playerInRange = false;
    CharacterController player;
    AudioManager audioManager;

    private void Start()
    {
        transform.position = startingWaypoint.transform.position;
        player = FindAnyObjectByType<CharacterController>();
        audioManager = FindAnyObjectByType<AudioManager>();
    }

    void Update()
    {
        
        if(!playerInRange)
        {
            Patrol();
        }
        else
        {
            Pursue();
        }

    }
    void DropRock()
    {
        Vector3 dropPosition = transform.position;
        dropPosition.y -= 0.5f;
        audioManager.PlaySFX("SFX_BirdCry");
        Instantiate(rockPrefab, dropPosition, Quaternion.identity);
        rockDropTimer = 0f;
    }

    void Patrol()
    {
        if(speed != 6)
        {
            speed = 6;
        }
        // Calculate the distance between the bird and its current target waypoint.
        float distanceToTarget = Vector3.Distance(transform.position, waypoints[targetWaypointIndex].point.transform.position);

        // If the bird is close enough to its target waypoint, move to the next waypoint.
        if (distanceToTarget < 1f)
        {
            targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
        }

        rockDropTimer += Time.deltaTime;

        // Calculate the direction to the target waypoint.
        Vector3 directionToTarget = (waypoints[targetWaypointIndex].point.transform.position - transform.position).normalized;

        // Move the bird in the direction of the target waypoint.
        transform.position += directionToTarget * speed * Time.deltaTime;

        // Rotate the bird towards the direction of the target waypoint.
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToTarget), rotationSpeed * Time.deltaTime);
    }


    void Pursue()
    {
        speed = 8f;
        // Calculate the distance between the bird and its current target waypoint.
        float distanceToTarget = Vector3.Distance(transform.position, player.transform.position);

        // Calculate the direction to the target waypoint.
        Vector3 directionToTarget = (player.transform.position - transform.position).normalized;

        directionToTarget.y = 0;

        // Move the bird in the direction of the target waypoint.
        transform.position += directionToTarget * speed * Time.deltaTime;

        rockDropTimer += Time.deltaTime;

        // Rotate the bird towards the direction of the target waypoint.
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToTarget), rotationSpeed * Time.deltaTime);

        if(distanceToTarget < dropDistance && (rockDropTimer >= rockDropInterval))
        {
            Invoke("DropRock", 0f);
        }
    }

     private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2)
            return;

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i].point.transform.position, waypoints[i + 1].point.transform.position);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
        
    }

}
[System.Serializable]
public class Waypoint
{
    public GameObject point;
    public bool isDropPoint;
}
