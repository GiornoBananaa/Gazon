using System;
using Game.Runtime.RhythmSystem;

namespace Game.Runtime.PianoFeature
{
    public class RhythmGameInstrumentKeyPresser : IInstrumentKeyPresser
    {
        private readonly IRhythmSheet _rhythmSheet;
        private int _keysCount;
        
        public RhythmGameInstrumentKeyPresser(IRhythmSheet rhythmSheet)
        {
            _rhythmSheet = rhythmSheet;
        }

        public event Action<int, int> OnPressedKeyNoteIndexes;
        public event Action<int, int> OnReleasedKeyNoteIndexes;

        public void SetKeysCount(int count)
        {
            _keysCount = count;
        }
        
        public void PressKey(int keyIndex)
        {
            keyIndex -= (GlobalConstants.MAX_KEYS_COUNT - _keysCount) / 2;
            if(keyIndex < 0 || keyIndex >= _keysCount) return;
            _rhythmSheet.StartKey(keyIndex);
        }

        public void ReleaseKey(int keyIndex)
        {
            keyIndex -= (GlobalConstants.MAX_KEYS_COUNT - _keysCount) / 2;
            if(keyIndex < 0 || keyIndex >= _keysCount) return;
            _rhythmSheet.StopKey(keyIndex);
        }
    }
}