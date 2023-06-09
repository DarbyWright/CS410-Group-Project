using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour {

    public string mus_name = "Hub";
    public int vari        = 0;
    public string amb_name = "Amb_BirdsSounds";
    public float volume    = 1f;
    public bool autoStart  = false;
    public bool onlyOnce   = true;


    // Play music on scene load
    void Start () {
        if (autoStart) {
            onlyOnce = true;
            SwitchMusic();
        }
    }


    // Player enters trigger
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
            SwitchMusic();
    }


    // Change which music or music variation is playing
    public void SwitchMusic() {

        // Play
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null) {
            audioManager.PlayMusic(mus_name, vari, amb_name);
        }

        // Volume
        //audioManager.MusicVolume(volume);

        // Only change music on the first collision
        if (onlyOnce) {
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }
}
