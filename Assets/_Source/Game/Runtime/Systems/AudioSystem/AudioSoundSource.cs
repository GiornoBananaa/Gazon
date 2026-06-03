using System;
using UnityEngine;

namespace Game.Runtime.AudioSystem
{
    public readonly struct AudioSoundSource : IEquatable<AudioSoundSource>
    {
        public readonly float MaxVolume;
        public readonly float MaxSpatialBlend;
        public readonly float MaxLowPassFrequency;
        
        private readonly int _audioSourceId;
       
        public AudioSoundSource(AudioSource audioSource, float maxVolume, float maxSpatialBlend, float maxLowPassFrequency)
        {
            _audioSourceId = audioSource.GetHashCode();
            MaxVolume = maxVolume;
            MaxSpatialBlend = maxSpatialBlend;
            MaxLowPassFrequency = maxLowPassFrequency;
        }
        
        public override int GetHashCode()
        {
            return _audioSourceId;
        }

        public bool Equals(AudioSoundSource other)
        {
            return _audioSourceId == other._audioSourceId;
        }

        public override bool Equals(object obj)
        {
            return obj is AudioSoundSource other && Equals(other);
        }
    }
}