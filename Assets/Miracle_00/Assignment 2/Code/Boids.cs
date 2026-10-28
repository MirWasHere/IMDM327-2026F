using UnityEngine;

public class Boids : MonoBehaviour
{
    // Prefab of boids
    [SerializeField] private GameObject leaves;

    public int numBoids = 10;
    public float radius = 10f;
    private float increments;
    public float fastTime = 3f;
    public float G = 500f;

    public float minimumDistance = 3f;
    public float maxDistance = 80f;
    public float separationForce = 10f;
    public float attractionForce = 10f;

    // Holds the moving bodies
    Flyer[] boids;

    // We'll use the midi numbers to decide what note is played
    [SerializeField] private MusicPlayer musPlay;
    // An array of all the midi numbers in order of musPlay's notes
    //                    {c, D, E, F, F#, G, G#, A, Bb, B, C};
    private int[] midis = {0, 2, 4, 5, 6, 7, 8,  9, 10, 11, 12};

    class Flyer
    {
        public GameObject body;
        public float mass;
        public Vector3 velocity;
        public Vector3 acceleration;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boids = new Flyer[numBoids];
        // Getting spacing between each boid
        increments = 360 / numBoids;
        // Creating the boids to be flung around!
        for(int i = 0; i < numBoids; i ++)
        {
            boids[i] = new Flyer();
            // Creating each boid in a circle shape
            float theta = i * increments;
            Vector3 initPos = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 100);

            boids[i].body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            boids[i].body.transform.position = initPos;
            //Instantiate(leaves, initPos, Quaternion.identity);
            boids[i].mass = Random.Range(1, 5);

            boids[i].velocity = Vector3.zero;


            // + This is just pretty trails
            TrailRenderer trailRenderer = boids[i].body.AddComponent<TrailRenderer>();
            // Configure the TrailRenderer's properties
            trailRenderer.time = 100.0f;  // Duration of the trail
            trailRenderer.startWidth = 0.5f;  // Width of the trail at the start
            trailRenderer.endWidth = 0.1f;    // Width of the trail at the end
            // a material to the trail
            trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
            // Set the colour gradient along the trail.
            Gradient gradient = new Gradient();
            Color targetColor = Color.HSVToRGB((float)i / numBoids, 1f, 1f);
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
        
    }

    // Update is called once per frame
    void Update()
    {
        // The radius of every other note will be determined by the midi number
        int notePlayed = (musPlay.GetNoteIndex() == -1 ? 0 : musPlay.GetNoteIndex());
        // Deciding whether to add or substract the midi from the radius
        bool negative = (Random.Range(0f, 1f) > 0.5? true : false);
        // Getting the midi note to change radius by
        int midi = midis[notePlayed] * (negative? -1 : 1); 

        for(int i = 0; i < numBoids; i ++)
        {
            boids[i].acceleration = Vector3.zero;
        }

        /*
        // Go through each boid and determine the new position
        for(int i = 0; i < numBoids; i ++)
        {
            float theta = i * increments;
            // Deciding whether to add or substract the midi from the radius
            bool negative = (Random.Range(0f, 1f) > 0.5? true : false);
            // Getting the midi note to change radius by
            int midi = midis[notePlayed] * (negative? -1 : 1); 

            // Getting the initial position of boid
            Vector3 initPos = boids[i].transform.position;

            // Getting new position
            Vector3 newVel = new Vector3((radius + midi) * Mathf.Cos(theta), (radius + midi) * Mathf.Sin(theta), 0) * Time.deltaTime * fastTime;

            boids[i].body.transform.position = newVel;
        }
        */

        for(int i = 0; i < numBoids; i ++)
        {
            for(int j = i + 1; j < numBoids; j ++)
            {
                Vector3 distance = boids[i].body.transform.position - boids[j].body.transform.position;
                Vector3 gravity = CalculateGravity(distance, boids[i].mass, boids[j].mass);

                boids[i].acceleration -= gravity / boids[i].mass;
                boids[j].acceleration += gravity / boids[j].mass;

                if(distance.magnitude < minimumDistance)
                {
                    boids[i].acceleration += separationForce * gravity / boids[i].mass;
                    boids[j].acceleration -= separationForce * gravity / boids[j].mass;
                }
                else if(distance.magnitude > maxDistance)
                {
                    boids[i].acceleration -= attractionForce * gravity / boids[i].mass;
                    boids[j].acceleration += attractionForce * gravity / boids[j].mass;
                }
            }
        }

        for(int i = 0; i < numBoids; i ++)
        {
            // Slow down the boids if they stop moving
            // if(musPlay.GetNoteIndex() == -1)
            // {
            //     boids[i].velocity -= boids[i].acceleration * Time.deltaTime * fastTime;
            // }
            // Otherwise, scale to the midi
            // else
            // {
                boids[i].velocity += boids[i].acceleration * Time.deltaTime * fastTime;
            // }
            
            boids[i].body.transform.position += boids[i].velocity * Time.deltaTime * fastTime;
        }
        
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
