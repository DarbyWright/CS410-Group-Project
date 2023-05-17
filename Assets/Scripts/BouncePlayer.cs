using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePlayer : MonoBehaviour {
    public float bounceDefaultStrength = 24f;
    public float bounceJumpStrength    = 36f;

    // Collide with objects
    void OnCollisionEnter(Collision collision) {

        // If player jumps on this object
        if (collision.gameObject.CompareTag("Player")) {
            PlayerController player = FindObjectOfType<PlayerController>();

            // Make sure player is above this object, then bounce it
            if (player.transform.position.y > transform.position.y)
                player.Bounce(bounceDefaultStrength, bounceJumpStrength);
        }
    }
}
