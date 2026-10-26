using UnityEngine;
using System.Collections;

public class HandTracker : MonoBehaviour
{
    // Max distance between finger top and base; what distance determines bend in each finger; current fing point distance
    private float[] maxDistanceLeft, maxDistanceRight, bentLeft, bentRight, currDistLeft, currDistRight;

    // Holds the mediapipe reference
    [SerializeField] private MediaPipeLandmarkDebug landmarks;

    // What fingers are down
    public bool[] fDownLeft, fDownRight;

    // Callibration checks
    private bool calLeft, calRight;
    public bool fullCal;

    private float thresholdRatio = 0.5f;

    void Start()
    {
        // Setting up some arrays...
        bentLeft = new float[4];
        bentRight = new float[4];

        fDownLeft = new bool[4];
        fDownRight = new bool[4];

        // ...and bools
        calLeft = false;
        calRight = false;
    }

    // Update is called once per frame
    void Update()
    {
        // For callibration, first we need to get the distances before 
        if(Input.GetKeyDown(KeyCode.RightShift))
        {
            maxDistanceLeft = GetDistances(landmarks.leftHandDots);
            calLeft = true;

            // The distance must be 1/2 of the total distance in order to be considered bent
            for(int i = 0; i < 4; i ++)
            {
                bentLeft[i] = maxDistanceLeft[i] * thresholdRatio;
            }
        }

        // Everything as above... except for the right hand
        else if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            maxDistanceRight = GetDistances(landmarks.rightHandDots);
            calRight = true;

            for(int i = 0; i < 4; i ++)
            {
                bentRight[i] = maxDistanceRight[i] * thresholdRatio;
            }
        }

        // Only check for fingerings if the user measure distances for both
        if(calLeft && calRight)
        {
            fullCal = true;
            // Check the distances each frame
            currDistLeft = GetDistances(landmarks.leftHandDots);
            currDistRight = GetDistances(landmarks.rightHandDots);

            // Go through each array to check if the fingers are bent
            for(int i = 0; i < 4; i ++)
            {
                // if the currDistance is less than the threshold distance, mark as bent
                if(currDistLeft[i] <= bentLeft[i])
                {
                    fDownLeft[i] = true;
                }
                else
                {
                    fDownLeft[i] = false;
                }

                if(currDistRight[i] <= bentRight[i])
                {
                    fDownRight[i] = true;
                }
                else
                {
                    fDownRight[i] = false;
                }
            }
        }
        
    }

    // Get the max distance between top finger point and bottom; callibration
    private float[] GetDistances(GameObject[] handDots)
    {
        // holds the distances from top to bottom each finger
        float[] distances = new float[4];
        
        // holds vectors for the top and bottom finger joints
        Vector3[] fingers = new Vector3[8];

        // finger index
        int index = 5;

        // Set each finger to a transform for ease
        for(int i = 0; i < 8; i += 2)
        {

            fingers[i] = handDots[index].transform.position;
            fingers[i + 1] = handDots[index + 3].transform.position;
            index += 4;
            
        }

        index = 0;
        // Calculate distances between top and bottom point on each finger
        for(int i = 0; i < 4; i ++)
        {
            distances[i] = Vector3.Distance(fingers[index], fingers[index + 1]);
            index += 2;
        }

        return distances;
    }
}
