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
        AudioManager.sfxVolume = sfxSlider.value / 100;
        AudioManager.musicVolume = sfxSlider.value / 100;
    }

    
}
