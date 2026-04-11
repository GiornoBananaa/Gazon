using Game.Runtime.AudioSystem;
using UnityEngine;

namespace Game.Runtime.PianoRhythmSystem
{
    public interface IPianoNoteTweener
    {
        void SetMaxVolume(float value);
        void SetPianoWorldPosition(Vector3 position);
        void SetSpatialBlend(float spatialBlend);
        void StartNote(int id, AudioSound sound);
        void EndNote(int id);
        void EnableSustain();
        void DisableSustain();
    }
}