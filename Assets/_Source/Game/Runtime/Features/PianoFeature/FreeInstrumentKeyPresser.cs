using System;
using Game.Runtime.AudioSystem;
using Game.Runtime.Configs;
using Game.Runtime.MusicInstrumentSystem;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class FreeInstrumentKeyPresser : IInstrumentKeyPresser
    {
        private readonly AudioSound[] _notes;
        private readonly IInstrumentNoteTweener _noteTweener;
        
        private readonly int _startKeyInFirstOctave;
        
        private int _keysCount;
        private int _startIndex;
        private int _startOctave;
        private int _octavesCount;
        private int _octavesInKeysCount;
        
        public event Action<int, int> OnPressedKeyNoteIndexes;
        public event Action<int, int> OnReleasedKeyNoteIndexes;
        
        public FreeInstrumentKeyPresser(IInstrumentNoteTweener noteTweener, PianoKeysConfig pianoKeysConfig)
        {
            _noteTweener = noteTweener;
            _notes = pianoKeysConfig.Notes;
            _startKeyInFirstOctave = pianoKeysConfig.StartKeyInFirstOctave;
            SetKeysCount(pianoKeysConfig.FreeKeysCount);
            SetOctave(_octavesCount / 2 - _octavesCount % 2);
        } 
        
        public void PressKey(int keyIndex)
        {
            int noteIndex = Mathf.Clamp(keyIndex + _startIndex, 0, _notes.Length - 1);
            _noteTweener.StartNote(noteIndex, _notes[noteIndex]);
            OnPressedKeyNoteIndexes?.Invoke(keyIndex, noteIndex);
        }
        
        public void ReleaseKey(int keyIndex)
        {
            int noteIndex = Mathf.Clamp(keyIndex + _startIndex, 0, _notes.Length - 1);
            _noteTweener.EndNote(noteIndex);
            OnReleasedKeyNoteIndexes?.Invoke(keyIndex, noteIndex); //TODO: key == note after octave move
        }
        
        public void OctaveUp()
        {
            if(_startOctave + _octavesInKeysCount > _octavesCount) return;
            SetOctave(_startOctave + 1);
        }

        public void OctaveDown()
        {
            if(_startOctave - 1 < (GlobalConstants.OCTAVE_NOTES_COUNT - _startKeyInFirstOctave > 0 ? -1 : 0)) return;
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
            int firstOctaveKeysCount = GlobalConstants.OCTAVE_NOTES_COUNT - _startKeyInFirstOctave;
            _octavesCount = (_notes.Length - firstOctaveKeysCount) / GlobalConstants.OCTAVE_NOTES_COUNT;
            _octavesInKeysCount = _keysCount / GlobalConstants.OCTAVE_NOTES_COUNT;
        }
        
        private void SetOctave(int octave)
        {
            _startOctave = Mathf.Clamp(octave, 0, _octavesCount);
            _startIndex = octave * GlobalConstants.OCTAVE_NOTES_COUNT + (GlobalConstants.OCTAVE_NOTES_COUNT - _startKeyInFirstOctave) + 1;
        }
    }
}