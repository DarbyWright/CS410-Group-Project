using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2SceneSwitch : MonoBehaviour
{
    FadeInOut fade;
    private int sceneNumber;
    GameManager gameManager;
    void Start()
    {
        sceneNumber = SceneManager.GetActiveScene().buildIndex;
        gameManager = FindAnyObjectByType<GameManager>();
        fade = FindAnyObjectByType<FadeInOut>();
    }


    public IEnumerator ChangeScene(int scene)
    {
        fade.FadeIn();
        yield return new WaitForSeconds(fade.timeToFade);
        SceneManager.LoadScene(scene);
    }

    void OnTriggerEnter(Collider other)
    {
        if (sceneNumber == 1)
        {
            StartCoroutine(ChangeScene(3));
        }
        else
        {
            StartCoroutine(ChangeScene(1));
        }
    }
}
