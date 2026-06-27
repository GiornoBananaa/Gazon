using Game.Runtime.AudioSystem;
using UnityEngine;

namespace Game.Runtime.MusicInstrumentSystem
{
    public interface IInstrumentNoteTweener
    {
        void SetMaxVolume(float value);
        void SetPianoWorldPosition(Vector3 start, Vector3 end);
        void SetSpatialBlend(float spatialBlend);
        void StartNote(int id, AudioSound sound, float velocity, float targetLength = 0);
        void EndNote(int id);
        void EnableSustain();
        void DisableSustain();
    }
}