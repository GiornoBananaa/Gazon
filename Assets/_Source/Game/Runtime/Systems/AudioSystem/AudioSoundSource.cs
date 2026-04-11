using System;
using UnityEngine;

namespace Game.Runtime.AudioSystem
{
    public readonly struct AudioSoundSource : IEquatable<AudioSoundSource>
    {
        public readonly float MaxVolume;
        public readonly float MaxSpatialBlend;
        
        private readonly int _audioSourceId;
       
        public AudioSoundSource(AudioSource audioSource, float maxVolume, float maxSpatialBlend)
        {
            _audioSourceId = audioSource.GetHashCode();
            MaxVolume = maxVolume;
            MaxSpatialBlend = maxSpatialBlend;
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