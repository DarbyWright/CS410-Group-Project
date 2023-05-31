using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrigger : MonoBehaviour
{
    public GameObject Spikes;
    public Transform ExtendPosition;
    public Transform RetractPosition;
    public bool moveObject = false;

    void FixedUpdate()
    {
        if (moveObject)
            Spikes.transform.position = Vector3.Lerp(RetractPosition.position, ExtendPosition.position, 100);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            moveObject = true;
        }
    }
}
