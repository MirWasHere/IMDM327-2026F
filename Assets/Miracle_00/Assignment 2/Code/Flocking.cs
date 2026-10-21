using UnityEngine;

public class Flocking : MonoBehaviour
{
    // Birds
    BodyProperty[] birds;
    public int flockSize = 10;
    [SerializeField] GameObject birdModel;


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
        // Instantiate the birds to have set mass and size
        for(int i = 0; i < flockSize; i ++)
        {
            birds[i] = new BodyProperty();
            birds[i].body = Instantiate(birdModel, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
