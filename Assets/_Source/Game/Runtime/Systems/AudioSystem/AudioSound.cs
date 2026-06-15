using UnityEngine;

namespace Game.Runtime.AudioSystem
{
    [CreateAssetMenu(fileName = "AudioSound", menuName = "Game/Audio")]
    public class AudioSound : ScriptableObject
    {
        public AudioClip AudioClip;
        [Range(0,1)] public float MaxVolume = 1;
        [Range(0,1)] public float MaxSpatialBlend = 0.9f;
        [Range(0,3)] public float Pitch = 1f;
        [Range(0,22000)] public float MaxLowPassFrequency = 22000f;
    }
}