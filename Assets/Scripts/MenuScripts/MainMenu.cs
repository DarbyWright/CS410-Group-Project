using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    FadeInOut fade;

    private void Start()
    {
        fade = FindAnyObjectByType<FadeInOut>();
    }

    public void Play()
    {
        StartCoroutine(ChangeScene());
    }

    public IEnumerator ChangeScene()
    {
        fade.FadeIn();
        yield return new WaitForSeconds(fade.timeToFade);
        Cursor.lockState = CursorLockMode.Locked;
        // Scenes are ordered in our dev order so Main Menu - > Hub -> Lvl 1
        // Mange scene swaps with index values
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
