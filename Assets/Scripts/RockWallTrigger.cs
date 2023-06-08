using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockWallTrigger : MonoBehaviour
{
    GameManager gameManager;
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.TNTExploded)
        {
            gameObject.SetActive(false);
        }
    }
}
