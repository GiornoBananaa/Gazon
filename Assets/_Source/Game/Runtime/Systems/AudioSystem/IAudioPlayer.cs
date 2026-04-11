using UnityEngine;

namespace Game.Runtime.AudioSystem
{
    public interface IAudioPlayer
    {
        AudioSoundSource Play(AudioSound sound, Vector3 point, float volume = 1.0f, float spatialBlend = 0);
        AudioSoundSource Play(AudioSound sound, Vector3 point, AudioSoundSource source, float volume = 1.0f, float spatialBlend = 0);
        void Stop(AudioSoundSource sound);
        float GetSoundVolume(AudioSoundSource sound);
        void SetSoundVolume(AudioSoundSource sound, float volume);
        void SetSpatialBlend(AudioSoundSource sound, float spatialBlend);
    }
}