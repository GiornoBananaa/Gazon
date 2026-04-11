using Game.Runtime.AudioSystem;
using UnityEngine;

namespace Game.Runtime.Configs
{
    [CreateAssetMenu(fileName = "PianoKeys", menuName = "Game/PianoKeys")]
    public class PianoKeysConfig : ScriptableObject
    {
        [Header("Free piano")]
        public int FreeKeysCount = 14;
        public float PedalNoteEndDuration = 4;
        
        [Header("Rhythm game")]
        public int MinRhythmKeys = 4;
        public int MaxRhythmKeys = 7;
        
        [Header("Notes")]
        public float NoteEndDuration = 1.5f;
        public float NoteTransitionFadingDuration = 0.05f;
        [Range(1, 12)] public int StartKeyInFirstOctave = 10;
        public AudioSound[] Notes;
    }
}