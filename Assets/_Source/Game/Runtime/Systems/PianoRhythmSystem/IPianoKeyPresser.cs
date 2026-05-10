using System;

namespace Game.Runtime.PianoRhythmSystem
{
    public interface IPianoKeyPresser
    {
        public event Action<int, int> OnPressedKeyNoteIndexes;
        public event Action<int, int> OnReleasedKeyNoteIndexes;
        
        void PressKey(int keyIndex);
        void ReleaseKey(int keyIndex);
        void OctaveUp();
        void OctaveDown();
        void PressPedal();
        void ReleasePedal();
    }
}