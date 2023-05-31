using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool Paused = false;
    public GameObject PauseMenuCanvas;
    public GameObject SettingsMenuCanvas;
    public GameObject ControlsMenuCanvas;
    bool inSettings = false;
    bool inControls = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (Paused && !inSettings && !inControls)
            {
                Play();
            }
            else if (Paused && inSettings) {
                inSettings = false;
                SettingsMenuCanvas.SetActive(false);
                Play();
            }
            else if (Paused && inControls)
            {
                inControls = false;
                ControlsMenuCanvas.SetActive(false);
                Play();
            }
            else
            {
                Pause();
            }
        }
    }

    void Pause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Paused = true;
        PauseMenuCanvas.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void SettingsSetter()
    {
        inSettings = !inSettings;
    }

    public void ControlsSetter()
    {
        inControls = !inControls;
    }

    public void Play()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Paused = false;

        PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void Quit()
    {
        Application.Quit();
    }

}
