using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BirdMovement : MonoBehaviour
{
    // Speed of the bird's movement in meters per second.
    public float speed = 2f;

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
    public float dropDistance = 1f;


    private void Start()
    {
        transform.position = startingWaypoint.transform.position;
    }

    void Update()
    {
        
        // Calculate the distance between the bird and its current target waypoint.
        float distanceToTarget = Vector3.Distance(transform.position, waypoints[targetWaypointIndex].point.transform.position);

        // If the bird is close enough to its target waypoint, move to the next waypoint.
        if (distanceToTarget < 0.1f)
        {
            targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;

            // Check if the new waypoint is a drop point.
            if (waypoints[targetWaypointIndex].isDropPoint)
            {
                DropRock();
            }
        }

        // Calculate the direction to the target waypoint.
        Vector3 directionToTarget = (waypoints[targetWaypointIndex].point.transform.position - transform.position).normalized;

        // Move the bird in the direction of the target waypoint.
        transform.position += directionToTarget * speed * Time.deltaTime;

        // Rotate the bird towards the direction of the target waypoint.
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToTarget), rotationSpeed * Time.deltaTime);
    }

    void DropRock()
    {
        // Instantiate a new rock object at the drop point.
        Vector3 dropPoint = waypoints[targetWaypointIndex].point.transform.position + (transform.up * dropDistance);
        Instantiate(rockPrefab, dropPoint, Quaternion.identity);
    }

}
[System.Serializable]
public class Waypoint
{
    public GameObject point;
    public bool isDropPoint;
}
