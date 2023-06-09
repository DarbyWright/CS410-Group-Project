using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    //Get sliders for music and sound effects
    public Slider musicSlider;
    public Slider sfxSlider;

    //Get text to display slider's current values
    public TextMeshProUGUI musicText;
    public TextMeshProUGUI sfxText;
    

    AudioManager musicManager;
    GameManager gameManager;

    private void Start()
    {
        musicManager = FindAnyObjectByType<AudioManager>();
        gameManager = FindAnyObjectByType<GameManager>();
        UpdateValue();
    }


    public void UpdateValue()
    {
        //Update values on screen
        musicText.text = musicSlider.value.ToString();
        sfxText.text = sfxSlider.value.ToString();

        //Pass along update to audio manager
        musicManager.SFXVolume(sfxSlider.value / 100);
        musicManager.MusicVolume(musicSlider.value / 100);
    }

    public void MuteMusic()
    {
        musicSlider.value = 0;
        musicText.text = musicSlider.value.ToString();
        musicManager.MusicVolume(0);
    }

    public void MuteSFX()
    {
        sfxSlider.value = 0;
        sfxText.text = sfxSlider.value.ToString();
        musicManager.SFXVolume(0);
    }

    
}
