using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class UIPopupController : MonoBehaviour
{
    public static bool Paused = false;
    public bool seenBefore = false;
    public List<GameObject> popupPages;


    private int currentPageIndex = 0;
    private bool checkForInput = false;
    private bool onScreen = false;
    private string uniqueID;

    private void Start()
    {
        Time.timeScale = 1.0f;
        uniqueID = SceneManager.GetActiveScene().ToString() + "_" + gameObject.name;
    }


    void Update()
    {
        //Check for input anytime player is within the trigger boundry
        if (checkForInput && Input.GetKeyDown(KeyCode.E))
        {
            //If there is a menu on the screen and it is the last in a list, close it
            if (currentPageIndex == popupPages.Count - 1 && onScreen)
            {
                ClosePopUp();
            }
            //If there is a menu on screen and there are more menus switch to the next one
            else if (currentPageIndex < popupPages.Count && onScreen)
            {
                popupPages[currentPageIndex].gameObject.SetActive(false);
                currentPageIndex = (currentPageIndex + 1) % popupPages.Count;
                OpenPopUp();
            }
            else
            {
                OpenPopUp();
            }
        }
    }


    void MarkSignAsSeen()
    {
        EditorPrefs.SetInt($"SignSeen_{uniqueID}", 1);
    }

    bool IsSignSeen()
    {
        int signSeen = EditorPrefs.GetInt($"SignSeen_{uniqueID}", 0);
        return signSeen == 1;
    }

    public void HandlePopUp()
    {
        if (!IsSignSeen())
        {
            MarkSignAsSeen();
            OpenPopUp();
        }
    }

    void OpenPopUp()
    {
        onScreen = true;
        popupPages[currentPageIndex].SetActive(true);
    }

    void ClosePopUp()
    {
        onScreen = false;
        popupPages[currentPageIndex].SetActive(false);
        currentPageIndex = 0;
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
        ClosePopUp();
    }
}



