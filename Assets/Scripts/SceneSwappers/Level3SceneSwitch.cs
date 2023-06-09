using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3SceneSwitch : MonoBehaviour
{
    FadeInOut fade;
    private int sceneNumber;
    GameManager gameManager;
    void Start()
    {
        sceneNumber = SceneManager.GetActiveScene().buildIndex;
        fade = FindAnyObjectByType<FadeInOut>();
        gameManager = FindAnyObjectByType<GameManager>();
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
            StartCoroutine(ChangeScene(4));
        }
        else
        {
            gameManager.UpdateAbilities(4);
            StartCoroutine(ChangeScene(1));
        }
    }
}
