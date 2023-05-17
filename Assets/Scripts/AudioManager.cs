using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    // AudioManager persists across scenes
    public static AudioManager instance;

    // Global sound volume
    [Range(0f, 1f)]
    public float sfxVolume   = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 1f;

    // Crossfade music volume
    static float crossfadeTime  = 120f;
    float crossfadeAmount       = 1f / crossfadeTime;
    float crossfadeVolumeTimer  = 0f;

    // All audio files in the game
    public Sound[] sfxSounds, musicTracks, ambientTracks;

    // Sources for the audio
    public AudioSource   sfxSource;
    public AudioSource[] musicSources;
    public AudioSource   ambientSource;

    // Which music track and variation of the music track is playing
    public string musicTrack;
    public int musicVar = -1;

    // Keep track of tracks and variations
    int variArrayLength = 6;
    int variHalfLength  = 6;
    bool isMusicTrackA  = true;
    int variOffset      = 0;


    // Awake is called before start - ininitialize sounds
    void Awake() {

        // Only one AudioManager at a time
        if (instance == null) {
            instance = this;

            // AudioManager exists across multiple scenes
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }
    }


    // Play a sound effect
    public void PlaySFX(string name) {

        // Find sound effect
        Sound sfx = Array.Find(sfxSounds, x => x.name == name);
        if (sfx == null) {
            Debug.Log("missing " + name);
            return;
        }

        // Harmonize to the music
        /*
        float pitch = sfx.pitch;
        bool quality = true;
        if (sfx.harmonize) {
            Tuple<float, bool> harmony = gameObject.GetComponent<Harmony>().harmonize(name, 0f);
            pitch *= harmony.Item1;
            quality = harmony.Item2;
        }
        */

        // Play
        /*
        sfxSource.clip   = sfx.clip;
        sfxSource.loop   = sfx.loop;
        sfxSource.volume = sfx.volume;
        sfxSource.pitch  = pitch;
        */
        sfxSource.PlayOneShot(sfx.clip, sfx.volume);
    }


    // Play a music (and possibly ambience) track
    public void PlayMusic(string mus_name, int vari, string amb_name) {
        //Debug.Log(mus_name + vari + amb_name);
        //Debug.Log(musicTrack + musicVar);

        // Don't play again if it's the same track and variation
        if (musicTrack == mus_name && musicVar == vari) {
            //Debug.Log(mus_name + " already playing");
            return;
        }

        // Change the variation of the music track
        //SwitchMusicVar(vari);

        // Don't play again if it's the same track
        if (musicTrack == mus_name) {
            //Debug.Log("switched to var " + vari + " of " + mus_name);
            return;
        }

        // Update which track and variation are playing
        musicTrack  = mus_name;
        musicVar    = vari;


        // Iterate through each varition of the current music track
        //for (int i = variOffset; i < variHalfLength + variOffset; i++) {
            int i = 0;

            // Find music tracks for each variation
            Sound music = Array.Find(musicTracks, x => x.name == mus_name); // + "_" + vari
            if (music == null)
                return;

            musicSources[i].Stop();
            ambientSource.Stop();

            // Play
            musicSources[i].clip   = music.clip;
            musicSources[i].loop   = music.loop;
            musicSources[i].volume = music.volume;
            //musicSources[i].pitch  = 2f;// + 2/3f;
            musicSources[i].Play();
        //}

        // Find ambience track
        Sound amb = Array.Find(ambientTracks, x => x.name == amb_name);
        if (amb != null) {
            ambientSource.clip   = amb.clip;
            ambientSource.loop   = amb.loop;
            ambientSource.volume = amb.volume;
            ambientSource.Play();
        }
    }


    // Change the variation of a music track
    public void SwitchMusicVar(int vari) {

        // Switch which track set is being used (A or B) for crossfading
        isMusicTrackA = !isMusicTrackA;
        variOffset = variHalfLength - variOffset;

        // Begin crossfade - see update()
        musicVar = vari;
        crossfadeVolumeTimer = crossfadeTime;
    }


    // Change sound effects volume
    public void SFXVolume(float volume) {
        sfxVolume = volume;
        sfxSource.volume = sfxVolume;
    }
    // Change music (and ambience) volume
    public void MusicVolume(float volume) {
        musicVolume = volume;

        // Iterate through all variAitions of the current music track
        foreach (AudioSource varSource in musicSources) {
            if (varSource.volume > 0)
                varSource.volume = musicVolume;
        }

        // Ambience
        ambientSource.volume = musicVolume;
    }


    // Change whether the sound effects are muted
    public void ToggleSFX(string name) {
        sfxSource.mute = !sfxSource.mute;
    }
    // Change whether the music (and ambience) is muted
    public void ToggleMusic() {

        // Music variations
        foreach (AudioSource varSource in musicSources)
            varSource.mute = !varSource.mute;

        // Ambience
        ambientSource.mute = !ambientSource.mute;
    }


    // Every frame - used for crossfading volume
    public void Update() {

        // Crossfade volume
        if (crossfadeVolumeTimer > 0) {

            // Iterate through each variAition of the current music track
            for (int i = 0; i < variArrayLength; i++) {
                AudioSource musVarTrack = musicSources[musicVar];
                if (musVarTrack == null) continue;

                // Increase the volume of the desired music variation
                if (i == musicVar && musVarTrack.volume < musicVolume &&
                (i >= variOffset && i < variOffset + variHalfLength))
                    musVarTrack.volume += crossfadeAmount * musicVolume;

                // Decrease the volume of all other music variations
                else if (musVarTrack.volume > 0)
                    musVarTrack.volume -= crossfadeAmount;
            }
        }
    }
}
