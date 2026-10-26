using System;
using System.Collections;
using System.Collections.Generic; // Contains TMPro
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private HandTracker hands;

    // Access UI for now to see what instrument is being pushed
    [SerializeField] private TextMeshProUGUI temp_fingerings;

    // All possible notes
    private string[] notes;

    /* 
        Finger patterns for each note; ascending order
        The left hand is first, written from inner to outter, and right
        hand is second, written inner to outer
    */
    private string[] fingPatts;


    // Check the code for notes to be played

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Possible Notes (big C is octave higher)
        notes = new string[] {"c", "D", "E", "F", "F#", "G", "G#", "A", "Bb", "E", "C"};

        // Finger patterns
        fingPatts = new string[]
                   {"1111 1111", // c
                    "1111 1110", // D
                    "1111 1100", // E
                    "1111 1000", // F
                    "1111 0010", // F#
                    "1111 0000", // G
                    "1101 0010", // G#
                    "1101 0000", // A
                    "1001 0010", // Bb
                    "1001 0000", // E
                    "0001 0000" // C
                    };

    }

    // Update is called once per frame
    void Update()
    {
        // Only run this program if it hands are fully callibrated
        if(hands.fullCal)
        {
            PrintNotes();
        }
    }


    private void PrintNotes()
    {
        string leftHand = "";
        string rightHand = "";
        string notePlayed = "";

        // Display what notes are being played
        for(int i = 0; i < 4; i ++)
        {
            leftHand += (hands.fDownLeft[i]? "1" : "0");
            rightHand += (hands.fDownRight[i]? "1" : "0");
        }

        // Check what note it is and add that, too
        for(int i = 0; i < notes.Length; i ++)
        {
            string combo = leftHand + " " + rightHand;

            // Debug.Log(combo);
            if(String.Compare(notes[i], combo) == 0)
            {
                Debug.Log($"Note played: {notes[i]}");
                notePlayed += notes[i];
                break;
            }
        }


        temp_fingerings.text = "LEFT: [ " + leftHand + " ] \nRIGHT: [ " + rightHand + " ]\nNOTE: " + notePlayed; 
    }
}
