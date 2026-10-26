using UnityEngine;
using System.Collections;

public class LoopingBeat : MonoBehaviour
{
    [SerializeField] public MediaPipeBodyTracker bdTrack;

    // ArrayList to track the distance between the beats 
    ArrayList beatDistance;

    float[] beats;

    // Timer for making the beat
    float timer = 0f;

    // Ensuring we don't run this twice over
    bool runningMB = false;

    AudioSource aud;


    void Start()
    {
        aud = GetComponent<AudioSource>();
    }


    // Update is called once per frame
    void Update()
    {
        // Only run if first beat is detected; allow player to intermingle pinches
        if(!runningMB && (bdTrack.leftHandPinch || bdTrack.rightHandPinch))
        {
            runningMB = true;
            StartCoroutine(MakeBeat());
        }
    }

    /*
        Makes beats based on the distance between pinches
        If the pinches are greater than 3s apart, end function (srry, no 20bpm here)
        Needs at least 2 beats to run
    */
    IEnumerator MakeBeat()
    { 
        // Reset timer
        timer = 0f;
        // Clear previous entries
        beatDistance = new ArrayList();

        // Add initial beat (with buffer)
        beatDistance.Add(0.5f);

        // To make sure pinching is counted on the first instance, not for holds
        bool pinching = false;

        // Continue to look while less than ~3s have passed
        while(timer <= 3.2f)
        {
            // Makeshift timer
            timer += Time.deltaTime;
            // if pinch is detected, add it to the list with time and restart timer
            if(!pinching && (bdTrack.leftHandPinch || bdTrack.rightHandPinch))
            {
                pinching = true;
                beatDistance.Add(timer);
                timer = 0.0f;
            }
            else
            {
                pinching = false;
            }

            yield return null;
        }

        runningMB = false;

        // moving beats to an array
        beats = (float[]) beatDistance.ToArray(typeof(float));

        // Once this coroutine is done, auto start playing the beat
        if(beatDistance.Count >= 2)
        {
            StartCoroutine(PlayBeat());
        }
    }

    IEnumerator PlayBeat()
    {      
        // if we're not pinching 
        while(!runningMB) 
        {
            // Go through each beat tracked   
            for(int i = 0; i < beats.Length; i ++)
            {
                // Play a beat!
                Debug.Log("Beat!" + i);
                aud.Play();
                
                // Wait for amount of time between notes
                yield return new WaitForSeconds(beats[i]);
            
            }
        }
    }
}
