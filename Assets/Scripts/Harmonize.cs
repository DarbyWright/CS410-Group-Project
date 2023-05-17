using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Harmony : MonoBehaviour {

    // Scales
    /*
    int[] majorScale         = { 0, 2, 4, 5, 7, 9, 11 };
    int[] minorScale         = { 0, 2, 3, 5, 7, 8, 10 };
    int[] minorScaleHarmonic = { 0, 2, 4, 5, 7, 8, 11 };
    int[] minorScaleMelodic  = { 0, 2, 4, 5, 7, 9, 11 };
    */


    Dictionary<string, int> notes = new Dictionary<string, int> {};
    Dictionary<string, List<string>> trackChords = new Dictionary<string, List<string>> {};


    // Harmonize a sound effect based on the current chord in the music track
    public Tuple<float, bool> harmonize(string musicTrack, float time) {
        int chordID = Mathf.FloorToInt(time);
        string chord = trackChords[musicTrack][chordID];

        string note    = chord.Substring(0, chord.Length - 1);
        string quality = chord.Substring(chord.Length - 1, 1);
        float semitonesToChange = notes[note];
        float newPitch = Mathf.Pow(2f, semitonesToChange / 12f);

        return new Tuple<float, bool>(1f, quality == "");
    }


    // Load music data
    void start() {

        // Notes
        notes["C"]  = 0;
        notes["C#"] = 1;
        notes["D"]  = 2;
        notes["Eb"] = 3;
        notes["E"]  = 4;
        notes["F"]  = 5;
        notes["F#"] = 6;
        notes["G"]  = 7;
        notes["Ab"] = 8;
        notes["A"]  = 9;
        notes["Bb"] = 10;
        notes["B"]  = 11;

        // Chords for each music track

        // Hub theme
        trackChords["Mus_Hub_Intro"] = new List<string>() { "G" };
        string[] Mus_Hub = {
            // Opening
            "C",  "C", "Dm", "G",
            "Em", "C", "G",  "G",
            "C",  "C", "Dm", "G",
            "Em", "C", "G",  "G",
            // Main melody
            "C", "G", "Am", "E",
            "F", "A", "Dm", "G",
            // Interlude
            "C", "Dm", "G", "Am",
            "F", "A",  "Dm", "G",
            // Main melody
            "C", "G", "Am", "E",
            "F", "A", "Dm", "G",
            // Motif
            "C", "D",  "D", "Em",
            "F", "Dm", "G", "C",
            // Bridge
            "C",  "C",  "G",  "G",
            "Am", "Gm", "A",  "Dm",
            "Dm", "C",  "Dm", "E",
            "Bb", "Dm", "G",  "C",
            // Motif
            "C", "D",  "D", "Em",
            "F", "Dm", "G", "C"
        };


        // Crystal cave theme
        string[] Level1_Chords = {
            "E", "C#m", "A", "B"
        };

        // Mine shaft theme
        string[] Level2_Chords = {
            "Cm", "Eb", "Ab", "G"
        };
    }
}
