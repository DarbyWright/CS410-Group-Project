using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject musicHolder;
    public void Play()
    {
        // Disable music prior to scene swapping as it will persist otherwise
        musicHolder.SetActive(false);

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
