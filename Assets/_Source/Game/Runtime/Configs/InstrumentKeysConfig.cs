using System;
using Game.Runtime.AudioSystem;
using UnityEngine;

namespace Game.Runtime.Configs
{
    [CreateAssetMenu(fileName = "PianoKeys", menuName = "Game/PianoKeys")]
    public class InstrumentKeysConfig : ScriptableObject
    {
        [Header("Free piano")]
        public int FreeKeysCount = 14;
        public float MaxKeyVelocity = 0.7f;
        public float VelocityRangeOnDevice = 0.7f;
        public float DefaultVelocity = 0.5f;
        public float PedalNoteEndDuration = 4;
        
        [Header("Rhythm game")]
        public int MinRhythmKeys = 4;
        public int MaxRhythmKeys = 7;
        
        [Header("Notes")]
        public float NoteEndDuration = 1.5f;
        public float NoteTransitionFadingDuration = 0.05f;
        public float NoteMinVelocityVolumeStartFadeDuration = 0.05f;
        [Range(1, 12)] public int StartKeyInFirstOctave = 10;
        public int StartNoteNumber = 21;
        public bool LoopNote;
        public NoteConfig[] Notes;

        [Header("Frequency")]
        public float MinLowPassFrequency = 1500f;
        public float MaxLowPassFrequency = 22000f;
    }

    [Serializable]
    public struct NoteConfig : IEquatable<NoteConfig>
    {
        public NoteType Note;
        public int Octave;
        public AudioSound Sound;

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Note, Octave);
        }

        public bool Equals(NoteConfig other)
        {
            return Note == other.Note && Octave == other.Octave;
        }

        public override bool Equals(object obj)
        {
            return obj is NoteConfig other && Equals(other);
        }
    }

    public enum NoteType
    {
        C = 0,
        Cs = 1,
        D = 2,
        Ds = 3,
        E = 4,
        F = 5,
        Fs = 6,
        G = 7,
        Gs = 8,
        A = 9,
        As = 10,
        B = 11,
    }
}