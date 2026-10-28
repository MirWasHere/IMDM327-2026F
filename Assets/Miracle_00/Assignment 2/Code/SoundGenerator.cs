// Discrete, polyphonic FM notes for a separate GameObject.
// A..' = white keys; Q..[ = black keys, with gaps at E, U and P.
using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundGenerator : MonoBehaviour
{
    [Header("Notes")]
    [Range(0, 108)] public int rootMidi = 48; // C3
    [Range(0f, 1f)] public float amplitude = 0.12f;
    [Header("FM Timbre")]
    [Range(0.1f, 8f)] public float modulatorRatio = 2f;
    [Range(0f, 20f)] public float modulationIndex = 1.5f;
    [Header("Envelope (seconds)")]
    [Range(0.001f, 1f)] public float attackSeconds = 0.01f;
    [Range(0.01f, 2f)] public float releaseSeconds = 0.18f;
    public bool showKeyboard = true;

    // static readonly KeyCode[] Keys =
    // {
            // White
        //    C3         D3          E3         F3         G3          A3       
    //     KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H,
        //   B3          C4        D4            E4                   F4
    //     KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Semicolon, KeyCode.Quote,

            // Black
        //    C#3        D#3                   F#3          G#3       A#3 
    //     KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y,
        //               C#4         D#4                         F#4
    //     KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P, KeyCode.LeftBracket
    // };

    // Will run function to see which note is being played
    [SerializeField] private MusicPlayer musPlay;

    // // All possible notes; in place of Keys
    //     "c", "D", "E", "F", "F#", "G", "G#", "A", "Bb", "B", "C"
    // //  0     1    2    3    14    4   15     5    16   6     7

    static readonly string[] Fingers = 
    {
        "c", "D", "E", "F", "G", "A", "B", "C", 
        "F#", "G#", "Bb"
    };

    // ONLY PLAY ONE NOTE AT A TIME!!!
    private bool noteIsPlaying = false;

    // Based on NoteNames index!
    static readonly int[] WhiteNotes = {0, 2, 4, 5, 7, 9, 11, 12};
    static readonly int[] BlackNotes = {6, 8, 10};

    static readonly string[] NoteNames =
        { "C", "D", "E", "F", "F#", "G", "G#", "A", "Bb", "B" };
    const double TwoPi = Math.PI * 2.0;

    struct Voice
    {
        public bool active, held;
        public double phase, modulatorPhase, frequency;
        public float envelope, releaseStep;
    }
    struct NoteEvent
    {
        public int key;
        public bool down;
        public float frequency;
    }
    struct Parameters
    {
        public float gain, ratio, index, attack, release;
    }

    // Main thread sends events; only the audio thread changes oscillator voices.
    readonly object eventLock = new object();
    readonly Queue<NoteEvent> events = new Queue<NoteEvent>(64);
    readonly Voice[] voices = new Voice[11];
    readonly bool[] heldKeys = new bool[11];
    readonly int[] heldMidi = new int[11];
    Parameters parameters;
    AudioSource source;
    int sampleRate;
    GUIStyle keyStyle;

    // Visual demos read the same notes used by the synthesizer.
    public bool IsNoteHeld(int key) => isActiveAndEnabled && heldKeys[key];

    public int GetNoteMidi(int key)
    {
        if (heldKeys[key]) return heldMidi[key];
        int offset = key < 11 ? WhiteNotes[key] : BlackNotes[key - 11];
        return offset < 0 ? -1 : Mathf.Clamp(rootMidi + offset, 0, 127);
    }

    public string GetNoteName(int key)
    {
        int midi = GetNoteMidi(key);
        return midi < 0 ? "" : NoteNames[midi % 12] + (midi / 12 - 1);
    }

    void Awake()
    {
        sampleRate = AudioSettings.outputSampleRate;
        source = GetComponent<AudioSource>();
        if (source == null) source = gameObject.AddComponent<AudioSource>();
        source.Stop();
        source.clip = null;
        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f;
        UpdateParameters();
        // Without a clip, this filter is the source (same as SoundFM).
        // Notes reach the next DSP buffer without a streaming clip's read-ahead.
    }

    void OnEnable()
    {
        if (source != null) source.Play();
    }

    void Update()
    {
        UpdateParameters();
        for (int i = 0; i < Fingers.Length; i++)
        {
            // Get whether it's the black or white note
            int offset = -1;

            if(i < 8)
            {
                Debug.Log("WHITE: " + i);
                offset = WhiteNotes[i];
            }
            else
            {
                Debug.Log("BLACK: " + (i - 8));
                offset = BlackNotes[i - 8];
            }

            // Find what input we had (and making sure it's not an unavailable note)
            if(!noteIsPlaying && string.Compare(musPlay.GetNote(), Fingers[i]) == 0)
            {
                int midi = Mathf.Clamp(rootMidi + offset, 0, 127);
                heldKeys[i] = true;
                heldMidi[i] = midi;
                Enqueue(i, true, 440f * Mathf.Pow(2f, (midi - 69) / 12f));

                // A note is playing, so we can't have any other notes play
                noteIsPlaying = true;
            }
            // Now we deque if it is released
            if (heldKeys[i] && string.Compare(musPlay.GetNote(), Fingers[i]) != 0)
            {
                heldKeys[i] = false;
                Enqueue(i, false, 0f);

                // No other notes are playing, free space
                noteIsPlaying = false;
            }
        }
    }


    void UpdateParameters()
    {
        lock (eventLock)
            parameters = new Parameters
            {
                gain = Mathf.Clamp01(amplitude), ratio = Mathf.Max(0.1f, modulatorRatio),
                index = Mathf.Max(0f, modulationIndex), attack = Mathf.Max(0.001f, attackSeconds),
                release = Mathf.Max(0.01f, releaseSeconds)
            };
    }

    void Enqueue(int key, bool down, float frequency)
    {
        lock (eventLock)
            events.Enqueue(new NoteEvent { key = key, down = down, frequency = frequency });
    }

    void ReleaseAll()
    {
        for (int i = 0; i < heldKeys.Length; i++)
        {
            heldKeys[i] = false;
            Enqueue(i, false, 0f);
        }
    }

    void OnApplicationFocus(bool focused)
    {
        if (!focused) ReleaseAll();
    }

    void OnDisable()
    {
        ReleaseAll();
        if (source != null) source.Stop();
    }

    // Generate one mono frame and copy it to every output channel.
    void OnAudioFilterRead(float[] data, int channels)
    {
        Parameters settings;
        lock (eventLock)
        {
            settings = parameters;
            while (events.Count > 0)
            {
                NoteEvent note = events.Dequeue();
                if (note.down)
                    voices[note.key] = new Voice { active = true, held = true, frequency = note.frequency };
                else
                {
                    voices[note.key].held = false;
                    voices[note.key].releaseStep = voices[note.key].envelope / (settings.release * sampleRate);
                }
            }
        }

        float attackStep = 1f / (settings.attack * sampleRate);
        for (int sample = 0; sample < data.Length; sample += channels)
        {
            double mix = 0.0;
            for (int i = 0; i < voices.Length; i++)
            {
                if (!voices[i].active) continue;
                Voice voice = voices[i];
                voice.envelope = voice.held ? Math.Min(1f, voice.envelope + attackStep)
                    : Math.Max(0f, voice.envelope - voice.releaseStep);
                if (!voice.held && voice.envelope <= 0f) voice.active = false;

                mix += Math.Sin(voice.phase + settings.index * Math.Sin(voice.modulatorPhase))
                    * voice.envelope * settings.gain;
                voice.phase = (voice.phase + TwoPi * voice.frequency / sampleRate) % TwoPi;
                voice.modulatorPhase = (voice.modulatorPhase + TwoPi * voice.frequency
                    * settings.ratio / sampleRate) % TwoPi;
                voices[i] = voice;
            }
            // Soft limiting keeps simultaneous notes below digital clipping.
            float output = (float)Math.Tanh(mix);
            for (int channel = 0; channel < channels; channel++)
                data[sample + channel] = output;
        }
    }
}
