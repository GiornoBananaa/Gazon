using System;
using Game.Runtime.AudioSystem;
using Game.Runtime.Configs;
using Game.Runtime.MusicInstrumentSystem;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class LimitedInstrumentKeyPresser : IInstrumentKeyPresser
    {
        private readonly NoteConfig[] _notes;
        private readonly IInstrumentNoteTweener _noteTweener;
        
        private readonly int _startKeyInFirstOctave;
        
        private int _keysCount;
        private int _startIndex;
        private int _startOctave;
        private int _octavesCount;
        private int _octavesInKeysCount;
        private float _velocity;
        
        public int KeysCount => _keysCount;
        public event Action<int, int> OnPressedKeyNoteIndexes;
        public event Action<int, int> OnReleasedKeyNoteIndexes;
        
        public LimitedInstrumentKeyPresser(IInstrumentNoteTweener noteTweener, InstrumentKeysConfig instrumentKeysConfig)
        {
            _noteTweener = noteTweener;
            _notes = instrumentKeysConfig.Notes;
            _startKeyInFirstOctave = instrumentKeysConfig.StartKeyInFirstOctave;
            _velocity = instrumentKeysConfig.DefaultVelocity;
            SetKeysCount(instrumentKeysConfig.FreeKeysCount);
        } 
        
        public void PressKey(int keyIndex)
        {
            int noteIndex = Mathf.Clamp(keyIndex + _startIndex, 0, _notes.Length - 1);
            _noteTweener.StartNote(noteIndex, _notes[noteIndex].Sound, _velocity);
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
            if(_startOctave - 1 < (_startKeyInFirstOctave > 0 ? -1 : 0)) return;
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
        
        public void SetKeysCount(int count)
        {
            _keysCount = count;
            int firstOctaveKeysCount = GlobalConstants.OCTAVE_NOTES_COUNT - _startKeyInFirstOctave;
            _octavesCount = (_notes.Length - firstOctaveKeysCount) / GlobalConstants.OCTAVE_NOTES_COUNT;
            if (firstOctaveKeysCount > 0)
                _octavesCount++;
            _octavesInKeysCount = _keysCount / GlobalConstants.OCTAVE_NOTES_COUNT;
            SetOctave((_octavesCount - _octavesInKeysCount) / 2 - (_octavesCount - _octavesInKeysCount) % 2);
        }
        
        private void SetOctave(int octave)
        {
            _startOctave = Mathf.Clamp(octave, 0, _octavesCount);
            _startIndex = octave * GlobalConstants.OCTAVE_NOTES_COUNT + (GlobalConstants.OCTAVE_NOTES_COUNT - _startKeyInFirstOctave) + 1;
        }
    }
}