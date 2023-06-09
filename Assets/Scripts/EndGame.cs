
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    GameManager gameManager;
    FadeInOut fade;
    float winScreenDuration = 15f;
    bool startEndSequence = false;
    public Canvas winScreen; 

    private void Start() {
        gameManager  = FindObjectOfType<GameManager>();
        fade = FindAnyObjectByType<FadeInOut>();
    }

    void Update()
    {
        if (startEndSequence)
        {
            winScreen.gameObject.SetActive(true);
            while (winScreenDuration > 0)
            {
                winScreenDuration -= Time.deltaTime;
            }
            StartCoroutine(FinishGame());
        }
    }

    public IEnumerator FinishGame(){
        fade.FadeIn();
        yield return new WaitForSeconds(fade.timeToFade * 2);
        SceneManager.LoadScene(5);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (gameManager.winCondition == true) {
                startEndSequence = true;
            }
        }
    }
}
