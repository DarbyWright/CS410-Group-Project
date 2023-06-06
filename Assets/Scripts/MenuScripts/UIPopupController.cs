using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopupController : MonoBehaviour
{
    public static bool Paused = false;
    public bool seenBefore = false;
    public GameObject PopupCanvas;
    public bool repeatable = true;
    bool onScreen = false;
    bool checkForInput = false;


    private void Start()
    {
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && onScreen)
        {
            ClosePopUp();
        }
        else if (checkForInput && Input.GetKeyDown(KeyCode.E) && repeatable)
        {
            OpenPopUp();
        }
    }


    public void HandlePopUp()
    {
        if(!seenBefore)
        {
            seenBefore = true;
            OpenPopUp();
        }
    }

    void OpenPopUp()
    {
        checkForInput = false;
        onScreen = true;
        PopupCanvas.SetActive(true);
    }


    void ClosePopUp()
    {
        onScreen = false;
        PopupCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            checkForInput = true;
            HandlePopUp();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        checkForInput = false;
    }
}
