using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    public Sound[] sounds;

    // AudioManager persists
    public static AudioManager instance;

    // Awake is called before start - ininitialize sounds
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

        foreach (Sound s in sounds) {
            s.source = gameObject.GetComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    public void Play(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        //s.source.Play();

        /*Sound sound = null;
        foreach (Sound s in sounds) {
            if (s.name == name) {
                sound = s;
                break;
            }
        }
        if (sound == null)
            return;
        sound.source.Play();
        */
    }

    // Start is called before the first frame update
    void Start() {
        //Play("Mus_Level_2");
    }

    // Update is called once per frame
    void Update() {

    }
}
