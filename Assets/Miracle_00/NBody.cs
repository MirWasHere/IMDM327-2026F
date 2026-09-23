// 3-body Starter Code
// Fall 2026. IMDM 327
// Instructor. Myungin Lee
using UnityEngine;
using System.Collections;
// using UnityEngine.InputSystem;


public class NBody : MonoBehaviour
{
    public float G = 500f; // Gravitational constant for this simulation, not the real-world value.
    public float minimumDistance = 3f;
    public float maxDistance = 80f;
    public float fasterTime = 3f;
    BodyProperty[] bp;
    // BodyProperty predator;
    public int numberOfSphere = 100;
    public float separationForce = 10f;
    public float attractionForce = 10f;

    class BodyProperty // why struct?
    {                   // https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct
        public GameObject body;
        public float mass;
        public Vector3 velocity;
        public Vector3 acceleration;
    }


    void Start()
    {
        // Allocate an array to store each body's properties.
        bp = new BodyProperty[numberOfSphere];

        // Creating a predator
        // predator = new BodyProperty();

        // Loop generating the gameobject and assign initial conditions (type, position, (mass/velocity/acceleration)
        for (int i = 0; i < numberOfSphere; i++)
        {
            // Our gameobjects are created here:
            bp[i] = new BodyProperty();
            bp[i].body = GameObject.CreatePrimitive(PrimitiveType.Sphere); // why sphere? try different options.
            // https://docs.unity3d.com/ScriptReference/GameObject.CreatePrimitive.html

            // initial conditions
            float r = 100f;
            float theta = (2 * Mathf.PI) / numberOfSphere * i;
            // position is (x,y,z). In this case, I want to plot them on the circle with r

            // ******** Fill in this part ********
            bp[i].body.transform.position = new Vector3(Mathf.Cos(theta) * r + Random.Range(-10f, 10f), Mathf.Sin(theta) * r + Random.Range(-10f, 10f), 180 + Random.Range(-10f, 10f));
            // z = 180 places the bodies in front of a camera near the origin looking along +Z. Try other positions too.

            bp[i].velocity = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)); // Try different initial condition
            bp[i].mass = 1; // Simplified. Try different initial condition


            // + This is just pretty trails
            TrailRenderer trailRenderer = bp[i].body.AddComponent<TrailRenderer>();
            // Configure the TrailRenderer's properties
            trailRenderer.time = 100.0f;  // Duration of the trail
            trailRenderer.startWidth = 0.5f;  // Width of the trail at the start
            trailRenderer.endWidth = 0.1f;    // Width of the trail at the end
            // a material to the trail
            trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
            // Set the colour gradient along the trail.
            Gradient gradient = new Gradient();
            Color targetColor = Color.HSVToRGB((float)i / numberOfSphere, 1f, 1f);
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.white, 0f), // (color, normalized position)
                    new GradientColorKey(targetColor, 0.8f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f), // (alpha, normalized position) 
                    new GradientAlphaKey(0f, 1f)
                }
            );
            trailRenderer.colorGradient = gradient;

        }
/*
        // Setting up the predator's intial position and speed
        // OIt will be a cube
        predator = new BodyProperty();
        predator.body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // Making it bigger
        predator.body.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        predator.body.transform.position = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
        // Setting a random start velocity
        predator.velocity = Vector3.zero;
        predator.mass = 3;
*/
    } 

    void Update()
    {
        // Loop for N-body gravity
        // How should we design the loop?
/*
        // for (int i = 0; i < numberOfSphere; i++)
        // {
        //     bp[i].acceleration = Vector3.zero;
        //     for(int j = 0; j < numberOfSphere - 1; j ++)
        //     {
        //         if( j != i)
        //         {
        //             // gravity
        //             Vector3 distance = bp[i].body.transform.position - bp[j].body.transform.position;
        //             Vector3 gravity = CalculateGravity(distance, bp[i].mass, bp[j].mass);
        //             // acceleration
        //             bp[i].acceleration -= gravity / bp[i].mass;
        //         }
        //     }
            
        //     // velocity
        //     bp[i].velocity += bp[i].acceleration * Time.deltaTime * 30f;
        //     // position
        //     bp[i].body.transform.position += bp[i].velocity * Time.deltaTime * 30f;
        // }
*/


        for(int i = 0; i < numberOfSphere; i ++)
        {
            bp[i].acceleration = Vector3.zero;
        }

        // Resetting predator acceleration
        // predator.acceleration = Vector3.zero;

        for(int i = 0; i < numberOfSphere; i ++)
        {
            for(int j = i + 1; j < numberOfSphere; j ++)
            {
                // gravity
                Vector3 distance = bp[i].body.transform.position - bp[j].body.transform.position;
                Vector3 gravity = CalculateGravity(distance, bp[i].mass, bp[j].mass);
                // acceleration
                bp[i].acceleration -= gravity / bp[i].mass;
                bp[j].acceleration += gravity / bp[j].mass;


                if(distance.magnitude < minimumDistance)
                {
                    bp[i].acceleration += separationForce * gravity / bp[i].mass;
                    bp[j].acceleration -= separationForce * gravity / bp[j].mass;
                }
                else if(distance.magnitude > maxDistance)
                {
                    bp[i].acceleration -= attractionForce * gravity / bp[i].mass;
                    bp[j].acceleration += attractionForce * gravity / bp[j].mass;
                }


            }
	/*
            // Setting the predator acceleration and gravity
            Vector3 distanceP = predator.body.transform.position - mousePos;
            Vector3 gravityP = CalculateGravity(distanceP, bp[i].mass, predator.mass);

            // Setting the predator to move towards the bps
            predator.acceleration += gravityP / predator.mass;
            bp[i].acceleration -= gravityP / bp[i].mass;

            // Moving the predator faster if far away from prey
            if(distanceP.magnitude > 50)
            {
                predator.acceleration += attractionForce * gravityP / predator.mass;
            }
            // Moving prey further away if close to predator
            else if(distanceP.magnitude < minimumDistance)
            {
                bp[i].acceleration += separationForce * 5f * gravityP / bp[i].mass;
            }
	*/

        }


        for(int i = 0; i < numberOfSphere; i ++)
        {
            // velocity
            bp[i].velocity += bp[i].acceleration * Time.deltaTime * fasterTime;
            // position
            bp[i].body.transform.position += bp[i].velocity * Time.deltaTime * fasterTime;
        }
	/*
        // Moving the predator
        predator.velocity += predator.acceleration * Time.deltaTime * fasterTime;
        predator.body.transform.position += predator.velocity * Time.deltaTime * fasterTime;
	*/
    }

    // Gravity Fuction to finish
    private Vector3 CalculateGravity(Vector3 distanceVector, float m1, float m2)
    {
        Vector3 gravity = Vector3.zero; // note this is also Vector3
                                        // **** Fill in the function below. 
        gravity = (G * m1 * m2) / distanceVector.sqrMagnitude * distanceVector.normalized;
        return gravity;
    }
}

