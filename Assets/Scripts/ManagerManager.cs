using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerManager : MonoBehaviour {

    void Awake() {

        // Get gameManager and audioManager
        GameManager gameManager  = FindObjectOfType<GameManager>();
        AudioManager audioManager = FindObjectOfType<AudioManager>();

        // If gameManager doesn't exist yet, create it
        if (gameManager == null) {
            GameManager gameManagerPrefab = Resources.Load<GameManager>("GameManager");
            gameManager = Instantiate(gameManagerPrefab, Vector3.zero, Quaternion.identity);
        }

        // If audioManager doesn't exist yet, create it
        if (audioManager == null) {
            AudioManager audioManagerPrefab = Resources.Load<AudioManager>("AudioManager");
            audioManager = Instantiate(audioManagerPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}
