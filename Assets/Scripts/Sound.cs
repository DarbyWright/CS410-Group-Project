using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound {

    public string name;
    public AudioClip clip;

    [HideInInspector]
    public AudioSource source;

    public bool loop = false;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(0.1f, 3f)] //[Range(-12f, 12f)]
    public float pitch = 1f;
}
