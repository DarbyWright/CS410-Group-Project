using System;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {

    // GameManager persists
    public static GameManager instance;

    /*
    Global variables
    */

    // inGame (not paused, cutscene, etc)
    public bool inGame;

    // Unlockalble abilities
    public bool hasDoubleJump = false;
    public bool hasDash       = false;
    public bool hasGlide      = false;

    // UI
    public TextMeshProUGUI UI;
    public int deathCount       = 0;
    public float totalGameTime  = 0f;


    // Awake is called before start - ininitialize
    void Awake() {

        // Only one AudioManager at a time
        if (instance == null)
            instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        // AudioManager exists across multiple scenes
        DontDestroyOnLoad(gameObject);

        // Lock abilities
        hasDoubleJump = false;
        hasDash       = false;
        hasGlide      = false;
    }


    // Start is called before the first frame update
    void Start() {

        // Game start
        inGame = true;

        // Get UI
        if (UI == null)
            UI = GetComponent<TextMeshProUGUI>();
    }


    // Update is called once per frame
    void Update() {

        // Add to time if "in game"
        if (inGame)
            totalGameTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(totalGameTime / 60f);
        int seconds = Mathf.FloorToInt(totalGameTime % 60f);
        int milliseconds = Mathf.FloorToInt((totalGameTime * 1000f) % 1000f);

        if (UI == null)
            return;

        // UI for debug
        UI.text = "- UI -\n";
        UI.text += "deaths: " + deathCount.ToString() + "\n";
        UI.text += "time: " + string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds) + "\n";
    }


    // Collecting the double jump item
    public void GotDoubleJump() {
        hasDoubleJump = true;
    }


    // Collecting the double jump item
    public void GotDash() {
        hasDash = true;
    }


    // Collecting the double jump item
    public void GotGlide() {
        hasGlide = true;
    }


    // Player dies
    public void PlayerDeath() {
        deathCount++;
    }
}
