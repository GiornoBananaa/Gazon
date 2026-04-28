using Game.Runtime.AudioSystem;
using Game.Runtime.Configs;
using UnityEngine;

namespace Game.Runtime.PianoRhythmSystem
{
    public class FreePianoKeyPresser : IPianoKeyPresser
    {
        private readonly AudioSound[] _notes;
        private readonly IPianoNoteTweener _noteTweener;
        
        private readonly int _startKeyInFirstOctave;
        private readonly int _octaveNotesCount = 12;
        
        private int _keysCount;
        private int _startIndex;
        private int _startOctave;
        private int _octavesCount;
        private int _octavesInKeysCount;
        
        public FreePianoKeyPresser(IPianoNoteTweener noteTweener, PianoKeysConfig pianoKeysConfig)
        {
            _noteTweener = noteTweener;
            _notes = pianoKeysConfig.Notes;
            _startKeyInFirstOctave = pianoKeysConfig.StartKeyInFirstOctave;
            SetKeysCount(pianoKeysConfig.FreeKeysCount);
            SetOctave(_octavesCount / 2 - _octavesCount % 2);
        } 
        
        public void PressKey(int index)
        {
            index = Mathf.Clamp(index + _startIndex, 0, _notes.Length - 1);
            _noteTweener.StartNote(index, _notes[index]);
        }
        
        public void ReleaseKey(int index)
        {
            index = Mathf.Clamp(index + _startIndex, 0, _notes.Length - 1);
            _noteTweener.EndNote(index);
        }
        
        public void OctaveUp()
        {
            if(_startOctave + _octavesInKeysCount > _octavesCount) return;
            SetOctave(_startOctave + 1);
        }

        public void OctaveDown()
        {
            if(_startOctave - 1 < (_octaveNotesCount - _startKeyInFirstOctave > 0 ? -1 : 0)) return;
            SetOctave(_startOctave - 1);
        }
        
        public void PressPedal()
        {
            _noteTweener.EnableSustain();
        }
        
        public void ReleasePedal()
        {
            _noteTweener.DisableSustain();
        }
        
        private void SetKeysCount(int count)
        {
            _keysCount = count;
            int firstOctaveKeysCount = _octaveNotesCount - _startKeyInFirstOctave;
            _octavesCount = (_notes.Length - firstOctaveKeysCount) / _octaveNotesCount;
            _octavesInKeysCount = _keysCount / _octaveNotesCount;
        }
        
        private void SetOctave(int octave)
        {
            _startOctave = Mathf.Clamp(octave, 0, _octavesCount);
            _startIndex = octave * _octaveNotesCount + (_octaveNotesCount - _startKeyInFirstOctave) + 1;
        }
    }
}