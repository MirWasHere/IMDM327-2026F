using UnityEngine;

public class Harmonizer : MonoBehaviour
{
    private string[] notes;

    // The gameobjects connected to the harmonizing bodies. HARMONIZERS WILL BE ONE BOID THAT WILL INTERMINGLE WITH
    // FLOCK WHILE BEING ON IT'S OWN PATTERN. Spice things up and make it easier. -_-
    [SerializeField] private GameObject harmony1, harmony2;
    private AudioSource harmony1_sound, harmony2_sound;

    Harmony[] harmonies;

    // "Tuple" of harmony types
    class Harmony
    {
        // Holds note played
        public string notePlayed;

        // Chord notes
        public string harm1;
        public string harm2;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Possible notes for the harmonizers
        notes = new string[] {"C", "C#", "D", "Eb", "E", "F", "F#", "G", "G#", "A", "Bb", "E"};

        // Audio sources
        harmony1_sound = harmony1.GetComponent<AudioSource>();
        harmony2_sound = harmony2.GetComponent<AudioSource>();

        harmonies = new Harmony[10];

        // -_-
        for(int i = 0; i < harmonies.Length; i ++)
        {
            harmonies[i] = new Harmony();
        }
        // Going through and writing up possible harmonies. By hand.
        
        harmonies[0].notePlayed = "C";
        harmonies[0].harm1 = "F";
        harmonies[0].harm2 = "G";

        harmonies[1].notePlayed = "D";
        harmonies[1].harm1 = "G";
        harmonies[1].harm2 = "A";

        harmonies[2].notePlayed = "E";
        harmonies[2].harm1 = "A";
        harmonies[2].harm2 = "B";

        harmonies[3].notePlayed = "F";
        harmonies[3].harm1 = "Bb";
        harmonies[3].harm2 = "C";

        harmonies[4].notePlayed = "F#";
        harmonies[4].harm1 = "B";
        harmonies[4].harm2 = "Db";

        harmonies[5].notePlayed = "G";
        harmonies[5].harm1 = "C";
        harmonies[5].harm2 = "D";

        harmonies[6].notePlayed = "G#";
        harmonies[6].harm1 = "Db";
        harmonies[6].harm2 = "Eb";

        harmonies[7].notePlayed = "A";
        harmonies[7].harm1 = "D";
        harmonies[7].harm2 = "E";

        harmonies[8].notePlayed = "Bb";
        harmonies[8].harm1 = "Eb";
        harmonies[8].harm2 = "F";

        harmonies[9].notePlayed = "B";
        harmonies[9].harm1 = "E";
        harmonies[9].harm2 = "F#";
    }


    // Plays the note based on the
    public void PlayHarmony(string notePlayed)
    {
        // Find the notes
        for(int i = 0; i < harmonies.Length; i ++)
        {
            // If we find a match, debug for now and break from loop
            if(string.Compare(harmonies[i].notePlayed, notePlayed) == 0)
            {
                Debug.Log($"Harmonizing... {harmonies[i].harm1} .. {harmonies[i].harm2}");
            }
        }
    }
}
