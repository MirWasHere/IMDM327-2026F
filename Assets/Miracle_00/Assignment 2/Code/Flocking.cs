using UnityEngine;
using System.Collections;

public class Flocking : MonoBehaviour
{
    // Birds
    BodyProperty[] birds;
    public int flockSize = 10;
    [SerializeField] GameObject birdModel;
    float theta;
    float distanceForce = 10f;
    float noteShifter;

    class BodyProperty 
    {
        public GameObject body;
        public float mass;
        public Vector3 velocity;
        public Vector3 acceleration;
    }
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        birds = new BodyProperty[flockSize];
        theta = Mathf.PI * flockSize / 360;
        // Instantiate the birds to have set mass and size
        for(int i = 0; i < flockSize; i ++)
        {
            // Randomizing the birds position in the flock
            float xPos = Random.Range(-5, 5);
            float yPos = Random.Range(-5, 5);
            // Creating the birds
            birds[i] = new BodyProperty();
            birds[i].body = Instantiate(birdModel, new Vector3(i, 0, 0), Quaternion.identity);

            // Setting masses
            birds[i].mass = 5;
            birds[i].body.transform.position = new Vector3(xPos, yPos, 0);

            // Setting velocities (for now they'll be randomized)
            birds[i].velocity = new Vector3(Random.Range(-5, 5), Random.Range(-5,5), 0);
            birds[i].acceleration = Vector3.zero;

        }
    }

    // Update is called once per frame
    void Update()
    {
        // for(int i = 0; i < flockSize; i ++)
        // {
        //     birds[i].acceleration = Vector3.zero;
        // }
        // for(int i = 0; i < flockSize; i ++)
        // {
        //     for(int j = i + 1; j < numberOfSphere; j ++)
        //     {
        //         // gravity
        //         Vector3 distance = birds[i].body.transform.position - birds[j].body.transform.position;
        //         Vector3 gravity = CalculateGravity(distance, birds[i].mass, birds[j].mass);
        //         // acceleration
        //         birds[i].acceleration -= gravity / birds[i].mass;
        //         birds[j].acceleration += gravity / birds[j].mass;


        //         if(distance.magnitude < 1)
        //         {
        //             birds[i].acceleration += distanceForce * gravity / birds[i].mass;
        //             birds[j].acceleration -= distanceForce * gravity / birds[j].mass;
        //         }
        //         else if(distance.magnitude > 10)
        //         {
        //             birds[i].acceleration -= distanceForce * gravity / birds[i].mass;
        //             birds[j].acceleration += distanceForce * gravity / birds[j].mass;
        //         }


        //     }
        //     // Adjusting the birds position
        //     birds[i].velocity += birds[i].acceleration;
        //     birds[i].body.transform.position += birds[i].velocity * Time.deltaTime ;


        // }

        // for(int i = 0; i < numberOfSphere; i ++)
        // {
        //     // velocity
        //     bp[i].velocity += bp[i].acceleration * Time.deltaTime * fasterTime;
        //     // position
        //     bp[i].body.transform.position += bp[i].velocity * Time.deltaTime * fasterTime;
        // }
    }

    // // Gravity Fuction to finish
    // private Vector3 CalculateGravity(Vector3 distanceVector, float m1, float m2)
    // {
    //     Vector3 gravity = Vector3.zero; // note this is also Vector3
    //                                     // **** Fill in the function below. 
    //     gravity = (G * m1 * m2) / distanceVector.sqrMagnitude * distanceVector.normalized;
    //     return gravity;
    // }
}
