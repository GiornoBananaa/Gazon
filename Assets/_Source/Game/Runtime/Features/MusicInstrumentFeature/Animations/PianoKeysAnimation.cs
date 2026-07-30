using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Runtime.RhythmSystem;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.MusicInstrumentFeature.Animations
{
    public interface IInstrumentKeysAnimation
    {
        public Vector3 GetPressPosition(int index);
    }
    
    public class PianoKeysAnimation : MonoBehaviour, IInstrumentKeysAnimation
    { 
        [SerializeField] private GameObject _whiteKey;
        [SerializeField] private GameObject _blackKey;
        [SerializeField] private int _keysCount = 88;
        [SerializeField] private int _startIndexInOctave = 10;
        [SerializeField] private float _whiteKeyWidth = 0.0235f;
        [SerializeField] private float _whiteKeyLength = 0.15f;
        [SerializeField] private float _keyGap = 0.001f;
        [SerializeField] private float _pressAngle = 5f;
        [SerializeField] private float _animationDuration = 0.1f;
        private readonly Dictionary<int, Tween> _pressedKeys = new();
        private Transform[] _keys;
        private IEnumerable<IInstrumentKeyPresser> _instrumentKeyPressers;
        private IRhythmSheet _rhythmSheet;
        
        public Vector3 GetPressPosition(int index)
        {
            index = Mathf.Clamp(index, 0, _keysCount-1);
            return _keys[index].position + new Vector3(0, 0, -_whiteKeyLength / 1.5f);
        }

        private void Awake()
        {
            _keys = new Transform[_keysCount];
            float xSize = 0;
            for (int i = 0; i < _keys.Length; i++)
            {
                bool white = true;
                int relativeKey = (i + _startIndexInOctave) % 12;
                switch (relativeKey)
                {
                    case 1:
                    case 3:
                    case 6:
                    case 8:
                    case 10:
                        white = false;
                        break;
                }
                if(white)
                    xSize += _whiteKeyWidth + _keyGap;
            }
            float x = 0;
            for (int i = 0; i < _keys.Length; i++)
            {
                bool white = true;
                int relativeKey = (i + _startIndexInOctave) % 12;
                switch (relativeKey)
                {
                    case 1:
                    case 3:
                    case 6:
                    case 8:
                    case 10:
                        white = false;
                        break;
                }
                _keys[i] = Instantiate(white ? _whiteKey : _blackKey, transform).transform;
                _keys[i].localPosition = new Vector3((white ? (x + (_whiteKeyWidth + _keyGap) / 2) : x) - xSize / 2, 0, 0);
                if(white)
                    x += _whiteKeyWidth + _keyGap;
            }
        }
        
        [Inject]
        public void Construct(IEnumerable<IInstrumentKeyPresser> instrumentKeyPressers, IRhythmSheet rhythmSheet)
        {
            UnsubscribeEvents();
            _instrumentKeyPressers = instrumentKeyPressers;
            
            if(_instrumentKeyPressers != null)
            {
                foreach (var keyPresser in _instrumentKeyPressers)
                {
                    keyPresser.OnPressedKeyNoteIndexes += OnPressedKeyNote;
                    keyPresser.OnReleasedKeyNoteIndexes += OnReleasedKeyNote;
                }
            }
            _rhythmSheet = rhythmSheet;
            if(_rhythmSheet != null)
            {
                _rhythmSheet.OnRhythmResult += OnPressedKeyRhythm;
                _rhythmSheet.OnRhythmEndResult += OnReleasedKeyRhythm;
            }
        }

        private void OnPressedKeyNote(int keyIndex, int noteIndex)
        {
            PressKey(noteIndex);
        }
        
        private void OnReleasedKeyNote(int keyIndex, int noteIndex)
        {
            ReleaseKey(noteIndex);
        }
        
        private void OnPressedKeyRhythm(RhythmKey rhythmKey, RhythmResult rhythmResult)
        {
            foreach (var note in rhythmKey.Notes)
            {
                PressKey(note.NoteNumber);
            }
        }

        private void OnReleasedKeyRhythm(RhythmKey rhythmKey, RhythmResult rhythmResult)
        {
            foreach (var note in rhythmKey.Notes)
            {
                ReleaseKey(note.NoteNumber);
            }
        }

        private void PressKey(int index)
        {
            if(index >= _keysCount) return;

            if(_pressedKeys.TryGetValue(index, out var tween))
                tween.Kill();
            _pressedKeys[index] = _keys[index].DOLocalRotate(new Vector3(-_pressAngle, 0, 0), _animationDuration);
        }
        
        private void ReleaseKey(int index)
        {
            if(index >= _keysCount) return;

            if(_pressedKeys.TryGetValue(index, out var tween))
                tween.Kill();
            _pressedKeys[index] = _keys[index].DOLocalRotate(new Vector3(0, 0, 0), _animationDuration);
        }
        
        private void UnsubscribeEvents()
        {
            if(_instrumentKeyPressers != null)
            {
                foreach (var keyPresser in _instrumentKeyPressers)
                {
                    keyPresser.OnPressedKeyNoteIndexes -= OnPressedKeyNote;
                    keyPresser.OnReleasedKeyNoteIndexes -= OnReleasedKeyNote;
                }
            }
            if (_rhythmSheet != null)
            {
                _rhythmSheet.OnRhythmResult -= OnPressedKeyRhythm;
                _rhythmSheet.OnRhythmEndResult -= OnReleasedKeyRhythm;
            }
        }
    }
}