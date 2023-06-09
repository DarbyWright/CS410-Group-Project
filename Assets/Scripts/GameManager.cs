using System;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    // GameManager persists across scenes
    public static GameManager instance;

    /*
    Global variables
    */

    // inGame (not paused, cutscene, etc)
    public bool inGame;

    //Walls in Lvl 2
    public bool TNTExploded = false;

    // Unlockalble abilities
    public bool hasDoubleJump = false;
    public bool hasDash       = false;
    public bool hasGlide      = false;

    // Stage MileStones
    public bool finishedCave = false;
    public bool finishedMine = false;
    public bool finishedCanyon = false;

    public bool winCondition = false;

    // UI
    public TextMeshProUGUI DeathCount;
    public TextMeshProUGUI TotalTime;
    public TextMeshProUGUI DoubleJumpStatus;
    public TextMeshProUGUI DashStatus;
    public TextMeshProUGUI GlideStatus;
    public int deathCount       = 0;
    public float totalGameTime  = 0f;
    public Vector3 respawnPos;
    public Vector3 sceneSwapRespawnPoint;
    public List<String> seenSigns;
    public AudioManager audioManager;

    // Awake is called before start - ininitialize
    void Awake() {

        // Only one GameManager at a time
        if (instance == null) {
            instance = this;

            // GameManager exists across multiple scenes
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }

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
        //if (UI == null)
        //    UI = GetComponent<TextMeshProUGUI>();
    }


    // Update is called once per frame
    void Update() {
        if (finishedCave && finishedMine && finishedCanyon) {
            print("Winning");
            winCondition = true;
        }

        // Add to time if "in game"
        if (inGame)
            totalGameTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(totalGameTime / 60f);
        int seconds = Mathf.FloorToInt(totalGameTime % 60f);
        int milliseconds = Mathf.FloorToInt((totalGameTime * 1000f) % 1000f);

        //if (UI == null)
          //  return;

        // UI for debug
        DeathCount.text = deathCount.ToString() + "\n";
        TotalTime.text = string.Format("{0:00}:{1:00}", minutes, seconds) + "\n";

        DoubleJumpStatus.text = (finishedCave ? "Collected" : "Missing") + "\n";
        DashStatus.text = (finishedMine ? "Collected" : "Missing") + "\n";
        GlideStatus.text =(finishedCanyon ? "Collected" : "Missing") + "\n";
        // UI.text = "- UI -\n";
        // UI.text += "deaths: " + deathCount.ToString() + "\n";
        // UI.text += "time: " + string.Format("{0:00}:{1:00}", minutes, seconds) + "\n";
        // UI.text += "current spawnpoint: X = " + respawnPos.x.ToString() + "Y = " + respawnPos.y.ToString() + "Z = " + respawnPos.z.ToString() + "\n";
        //UI.text += "time: " + string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds) + "\n";
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

    public void GotCave()
    {
        finishedCave = true;
    }
    public void GotMine()
    {
        finishedMine = true;
    }

    public void GotCanyon()
    {
        finishedCanyon = true;
    }

    // Player dies
    public void PlayerDeath() {
        Debug.Log("deathCount: " + deathCount);
        deathCount++;
    }

    public void SetSpawn(Vector3 newRespawnPos) {
        respawnPos = newRespawnPos;
        Debug.Log("set new spawn: " + respawnPos.x + " " + respawnPos.x + " " + respawnPos.z);
    }

    public void SetSpawnForSceneSwap(Vector3 newRespawnPos)
    {
        sceneSwapRespawnPoint = newRespawnPos;

    }

    public void UpdateAbilities(int currentScene)
    {
        switch (currentScene)
        {
            case 2:
                hasDoubleJump = finishedCave;
                break;
            case 3:
                hasDash = finishedMine;
                break;
            case 4:
                hasGlide = finishedCanyon;
                break;
        }
    }
}
