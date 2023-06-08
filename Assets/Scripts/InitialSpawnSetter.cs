using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialSpawnSetter : MonoBehaviour
{
    GameManager gameManager;
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();   
    }
    private void OnTriggerEnter(Collider other)
    {
        gameManager.SetSpawn(transform.position);
    }
}
