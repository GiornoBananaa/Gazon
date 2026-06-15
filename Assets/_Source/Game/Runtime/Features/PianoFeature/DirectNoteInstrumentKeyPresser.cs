using System;
using Game.Runtime.AudioSystem;
using Game.Runtime.Configs;
using Game.Runtime.MusicInstrumentSystem;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class DirectNoteInstrumentKeyPresser : IInstrumentKeyPresser
    {
        private readonly NoteConfig[] _notes;
        private readonly IInstrumentNoteTweener _noteTweener;
        
        private int _keysCount;
        private int _startIndex;
        private int _startOctave;
        private int _octavesCount;
        private int _octavesInKeysCount;
        private float _velocityRange;
        
        public event Action<int, int> OnPressedKeyNoteIndexes;
        public event Action<int, int> OnReleasedKeyNoteIndexes;
        
        public DirectNoteInstrumentKeyPresser(IInstrumentNoteTweener noteTweener, InstrumentKeysConfig instrumentKeysConfig)
        {
            _noteTweener = noteTweener;
            _notes = instrumentKeysConfig.Notes;
            _velocityRange = instrumentKeysConfig.VelocityRangeOnDevice;
        } 
        
        public void PressKey(int keyIndex, float velocity)
        {
            velocity = Mathf.InverseLerp(0, _velocityRange, velocity);
            _noteTweener.StartNote(keyIndex, _notes[keyIndex].Sound, velocity);
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