using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3SceneSwitch : MonoBehaviour
{
    private int sceneNumber;
    void Start()
    {
        sceneNumber = SceneManager.GetActiveScene().buildIndex;
    }

    void OnTriggerEnter(Collider other)
    {
        if (sceneNumber == 1)
        {
            SceneManager.LoadScene(4);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }
}
