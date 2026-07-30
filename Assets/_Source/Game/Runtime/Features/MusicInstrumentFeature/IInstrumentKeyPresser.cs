using System;

namespace Game.Runtime.MusicInstrumentFeature
{
    public interface IInstrumentKeyPresser
    {
        public event Action<int, int> OnPressedKeyNoteIndexes;
        public event Action<int, int> OnReleasedKeyNoteIndexes;
    }
}