using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    GameManager gameManager;
    FadeInOut fade;

    private void Start() {
        gameManager  = FindObjectOfType<GameManager>();
        fade = FindAnyObjectByType<FadeInOut>();
    }

    public IEnumerator ChangeScene(int scene){
        fade.FadeIn();
        yield return new WaitForSeconds(fade.timeToFade);
        SceneManager.LoadScene(scene);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (gameManager.winCondition == true) {
                StartCoroutine(ChangeScene(5));
            }
        }
    }
}
