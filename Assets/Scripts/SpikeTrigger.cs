using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrigger : MonoBehaviour
{
    public GameObject Spikes;
    public Transform ExtendPosition;
    public Transform RetractPosition;
    public bool moveObject = false;

    private AudioManager audioManager;
    private int extendedDurationMax = 60;
    private int extendedDuration = 0;


    private void Start()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
    }

    void FixedUpdate()
    {
        if (moveObject) // Extend
        {
            Invoke("SpikeDelay", 0.3f);
            audioManager.PlaySFX("SFX_SpikeExtend");
        }
        else { // Retract
            Spikes.transform.position = Vector3.Lerp(ExtendPosition.position, RetractPosition.position, 1000);
            audioManager.PlaySFX("SFX_SpikeRetract");
        }
        
        // Check if extended
        if (extendedDuration > 0) {
            extendedDuration--;
        }
        else { // if been extended long enough, retract
            moveObject = false;
        }
    }

    void SpikeDelay()
    {
        Spikes.transform.position = Vector3.Lerp(RetractPosition.position, ExtendPosition.position, 1000);
    }

    void OnTriggerEnter(Collider other)
    {
        // If collided with
        if (other.gameObject.CompareTag("Player"))
        {
            moveObject = true;
            extendedDuration = extendedDurationMax;
        }
    }
}
