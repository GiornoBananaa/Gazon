using Game.Runtime.AudioSystem;
using Game.Runtime.Configs;
using UnityEngine;

namespace Game.Runtime.PianoRhythmSystem
{
    public class RhythmGamePianoKeyPresser : IPianoKeyPresser
    {
        private readonly AudioSound[] _notes;
        private readonly IPianoNoteTweener _noteTweener;
        
        private readonly int _startKeyInFirstOctave;
        private readonly int _octaveNotesCount = 7;
        
        private int _keysCount;
        private int _startIndex;
        private int _startOctave;
        private int _octavesCount;
        private int _octavesInKeysCount;
        
        public RhythmGamePianoKeyPresser(IPianoNoteTweener noteTweener, PianoKeysConfig pianoKeysConfig)
        {
            _noteTweener = noteTweener;
            _notes = pianoKeysConfig.Notes;
        }

        public void PressKey(int index)
        {
            throw new System.NotImplementedException();
        }

        public void ReleaseKey(int index)
        {
            throw new System.NotImplementedException();
        }

        public void SetKeysCount(int count)
        {
            throw new System.NotImplementedException();
        }

        public void OctaveUp()
        {
            throw new System.NotImplementedException();
        }

        public void OctaveDown()
        {
            throw new System.NotImplementedException();
        }

        public void PressPedal()
        {
            throw new System.NotImplementedException();
        }

        public void ReleasePedal()
        {
            throw new System.NotImplementedException();
        }
    }
}