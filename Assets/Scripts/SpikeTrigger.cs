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
            moveObject = false;
            Invoke("SpikeExtend", 0.25f);
            
        }
        
        // Check if extended
        if (extendedDuration > 0) {
            extendedDuration--;
        }
        else { // if been extended long enough, retract
            moveObject = false;
        }
    }

    void SpikeExtend()
    {
        audioManager.PlaySFX("SFX_SpikeExtend");
        Spikes.transform.position = Vector3.Lerp(RetractPosition.position, ExtendPosition.position, 1000);
        Invoke("SpikeRetract", 1.5f);
    }

    void SpikeRetract()
    {
        audioManager.PlaySFX("SFX_SpikeRetract");
        Spikes.transform.position = Vector3.Lerp(ExtendPosition.position, RetractPosition.position, 1000);
    }

    void OnTriggerEnter(Collider other)
    {
        // If collided with
        if (other.gameObject.CompareTag("Player"))
        {
            if (extendedDuration <= 0) {
                moveObject = true;
                extendedDuration = extendedDurationMax;
            }
        }
    }
}
