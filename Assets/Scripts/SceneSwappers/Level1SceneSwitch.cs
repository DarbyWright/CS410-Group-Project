using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1SceneSwitch : MonoBehaviour
{
    FadeInOut fade;
    private int sceneNumber;
    void Start()
    {
        sceneNumber = SceneManager.GetActiveScene().buildIndex;
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
            StartCoroutine(ChangeScene(2));
        }
        else
        {
            StartCoroutine(ChangeScene(1));
        }
    }
}
