using UnityEngine;

public class BirdController : MonoBehaviour
{
    public Transform centerPoint;  // Center point of the designated sphere
    public float sphereRadius = 10f;  // Radius of the designated sphere
    public float flyingSpeed = 5f;  // Speed at which the bird flies
    public float dropRange = 8f;  // Range at which the bird drops rocks
    public GameObject rockPrefab;  // Prefab of the rock object to be dropped

    private bool isPlayerInRange = false;
    private Vector3 targetPosition;  // Target position within the designated sphere

    private void Start()
    {
        // Set the initial target position
        SetRandomTargetPosition();
    }

    private void Update()
    {
        
        while(Vector3.Distance(transform.position, targetPosition) < 3f)
        {
            SetRandomTargetPosition();
        }

        targetPosition.y = transform.position.y;
        // Rotate the bird towards the target position
        transform.rotation = Quaternion.LookRotation(targetPosition - transform.position);

        // Move the bird towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, flyingSpeed * Time.deltaTime);

        // Check if the bird has reached the target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Set a new random target position
            SetRandomTargetPosition();
        }

        // Check if the player is within range
        if (isPlayerInRange)
        {
            // Drop a rock
            DropRock();
        }
    }

    private void DropRock()
    {
        // Instantiate a rock object at the bird's position
        Instantiate(rockPrefab, transform.position, Quaternion.identity);

        // Reset the range flag to avoid dropping multiple rocks continuously
        isPlayerInRange = false;
    }

    private void SetRandomTargetPosition()
    {
        // Calculate a random point within the designated sphere
        targetPosition = centerPoint.position + Random.insideUnitSphere * sphereRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
