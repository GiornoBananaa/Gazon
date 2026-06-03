using System;
using Game.Runtime.AudioSystem;
using Game.Runtime.Configs;
using Game.Runtime.MusicInstrumentSystem;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class DirectNoteInstrumentKeyPresser : IInstrumentKeyPresser
    {
        private readonly AudioSound[] _notes;
        private readonly IInstrumentNoteTweener _noteTweener;
        
        private int _keysCount;
        private int _startIndex;
        private int _startOctave;
        private int _octavesCount;
        private int _octavesInKeysCount;
        
        public event Action<int, int> OnPressedKeyNoteIndexes;
        public event Action<int, int> OnReleasedKeyNoteIndexes;
        
        public DirectNoteInstrumentKeyPresser(IInstrumentNoteTweener noteTweener, PianoKeysConfig pianoKeysConfig)
        {
            _noteTweener = noteTweener;
            _notes = pianoKeysConfig.Notes;
        } 
        
        public void PressKey(int keyIndex, float velocity)
        {
            _noteTweener.StartNote(keyIndex, _notes[keyIndex], velocity);
            OnPressedKeyNoteIndexes?.Invoke(keyIndex, keyIndex);
        }

        public void ReleaseKey(int keyIndex)
        {
            int noteIndex = Mathf.Clamp(keyIndex + _startIndex, 0, _notes.Length - 1);
            _noteTweener.EndNote(noteIndex);
            OnReleasedKeyNoteIndexes?.Invoke(keyIndex, noteIndex);
        }

        public void PressPedal()
        {
            _noteTweener.EnableSustain();
        }
        
        public void ReleasePedal()
        {
            _noteTweener.DisableSustain();
        }
    }
}