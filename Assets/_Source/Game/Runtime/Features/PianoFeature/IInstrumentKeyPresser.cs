using System;

namespace Game.Runtime.PianoFeature
{
    public interface IInstrumentKeyPresser
    {
        public int KeysCount { get; }
        
        public event Action<int, int> OnPressedKeyNoteIndexes;
        public event Action<int, int> OnReleasedKeyNoteIndexes;
        
        void PressKey(int keyIndex);
        void ReleaseKey(int keyIndex);
    }
}