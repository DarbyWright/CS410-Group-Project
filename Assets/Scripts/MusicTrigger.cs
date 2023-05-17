using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour {

    public string mus_name = "Mus_Hub_1";
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
        if (other.CompareTag("Player")) {
            //Debug.Log("touched");
            //SwitchMusic();
        }
    }


    // Change which music or music variation is playing
    public void SwitchMusic() {
        //Debug.Log("switch to " + mus_name + vari + amb_name);

        // Play
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
            audioManager.PlayMusic(mus_name, vari, amb_name);

        // Volume
        //audioManager.MusicVolume(volume);

        // Only change music on the first collision
        if (onlyOnce) {
            //Debug.Log("deactivate " + mus_name + vari + amb_name);
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }
}
