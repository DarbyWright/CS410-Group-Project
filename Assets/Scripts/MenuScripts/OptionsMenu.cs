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

    private void Start()
    {
        UpdateValue();
    }


    public void UpdateValue()
    {
        //Update values on screen
        musicText.text = musicSlider.value.ToString();
        sfxText.text = sfxSlider.value.ToString();

        //Pass along update to audio manager
        AudioManager.instance.SFXVolume(sfxSlider.value / 100);
        AudioManager.instance.MusicVolume(musicSlider.value / 100);
    }

    public void MuteMusic()
    {
        musicSlider.value = 0;
        musicText.text = musicSlider.value.ToString();
        AudioManager.instance.MusicVolume(0);
    }

    public void MuteSFX()
    {
        sfxSlider.value = 0;
        sfxText.text = sfxSlider.value.ToString();
        AudioManager.instance.SFXVolume(0);
    }

    
}
