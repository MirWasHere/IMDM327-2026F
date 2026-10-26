using UnityEngine;
using System.Collections;

public class HandTracker : MonoBehaviour
{
    // Max distance between finger top and base; what distance determines bend in each finger
    public float[] maxDistanceLeft, maxDistanceRight, bentLeft, bentRight;

    // Holds the mediapipe reference
    [SerializeField] private MediaPipeLandmarkDebug landmarks;

    // What fingers are down
    public bool[] fDownLeft, fDownRight;

    // Callibration checks
    public bool calLeft = false, calRight = false;

    void Start()
    {
        bentLeft = new float[4];
        bentRight = new float[4];

        fDownLeft = new bool[4];
        fDownRight = new bool[4];
    }

    // Update is called once per frame
    void Update()
    {
        // For callibration, first we need to get the distances before 
        if(Input.GetKeyDown(KeyCode.RightShift))
        {
            maxDistanceLeft = GetDistances(landmarks.leftHandDots);
            calLeft = true;

            Debug.Log("bent left length: " + bentLeft.Length);

            // The distance must be 1/3 of the total distance in order to be considered bent
            for(int i = 0; i < 4; i ++)
            {
                bentLeft[i] = maxDistanceLeft[i] * 0.3f;
            }
        }

        else if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            maxDistanceRight = GetDistances(landmarks.rightHandDots);
            calRight = true;

            // The distance must be 1/3 of the total distance in order to be considered bent
            for(int i = 0; i < 4; i ++)
            {
                bentRight[i] = maxDistanceRight[i] * 0.3f;
            }
        }

        // Only check for fingerings if the user measure distances for both
        if(calLeft && calRight)
        {
            // Go through each array to check if the fingers are bent

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
