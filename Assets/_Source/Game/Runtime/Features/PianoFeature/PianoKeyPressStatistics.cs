using System;
using System.Collections.Generic;
using Game.Runtime.Configs;
using Game.Runtime.PianoRhythmSystem;
using Game.Runtime.ServiceSystem;
using R3;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class PianoKeyPressStatistics : IUpdatable, IDisposable
    {
        private class KeyStatisticEvent
        {
            public readonly bool Pressed;
            public readonly int Key;
            public readonly int Note;
            public readonly float Tone;
            public float ElapsedTime;

            public KeyStatisticEvent(bool pressed, int key, int note, float tone)
            {
                Pressed = pressed;
                Key = key;
                Note = note;
                Tone = tone;
                ElapsedTime = 0;
            }
        }
        
        private readonly ReactiveProperty<int> _notePressedCount = new();
        private readonly ReactiveProperty<int> _notePressCountOverTime = new();
        private readonly ReactiveProperty<float> _averageTone = new();
        private readonly ReactiveProperty<float> _averageToneOverTime = new();
        private readonly ReactiveProperty<float> _lastTone = new();
        private readonly ReactiveProperty<float> _lastNoteLength = new();
        private readonly ReactiveProperty<float> _averageNoteLength = new();
        private readonly ReactiveProperty<float> _silenceLength = new();
        
        public ReadOnlyReactiveProperty<int> NotePressedCount => _notePressedCount;
        public ReadOnlyReactiveProperty<int> NotePressCountOverTime => _notePressCountOverTime;
        public ReadOnlyReactiveProperty<float> AverageTone => _averageTone;
        public ReadOnlyReactiveProperty<float> AverageToneOverTime => _averageToneOverTime;
        public ReadOnlyReactiveProperty<float> LastTone => _lastTone;
        public ReadOnlyReactiveProperty<float> SilenceLength => _silenceLength;
        public ReadOnlyReactiveProperty<float> LastNoteLength => _lastNoteLength;
        public ReadOnlyReactiveProperty<float> AverageNoteLength => _averageNoteLength;
        
        private readonly Queue<KeyStatisticEvent> _statisticEvents = new();
        private readonly HashSet<int> _currentPressedKeys = new();
        private readonly HashSet<int> _currentPressedNotes = new();
        private readonly int _maxNotes;
        
        private IPianoKeyPresser _pianoKeyPresser;
        
        public float MaxTimeSpan { get; private set; }

        
        public PianoKeyPressStatistics(PianoKeysConfig config)
        {
            MaxTimeSpan = config.StatisticTimeSpan;
            _maxNotes = config.Notes.Length;
        }
        
        public void SetPianoKeyPresser(IPianoKeyPresser pianoKeyPresser)
        {
            if (_pianoKeyPresser != null)
                UnsubscribeEvents();
            _pianoKeyPresser = pianoKeyPresser;
            _pianoKeyPresser.OnPressedKeyNoteIndexes += OnPressedKeyNote;
            _pianoKeyPresser.OnReleasedKeyNoteIndexes += OnReleasedKeyNote;
        }

        private void OnPressedKeyNote(int keyIndex, int noteIndex)
        {
            _statisticEvents.Enqueue(new KeyStatisticEvent(true, keyIndex, noteIndex, (float)noteIndex/_maxNotes));
            _currentPressedKeys.Add(keyIndex);
            _currentPressedNotes.Add(noteIndex);
        }
        
        private void OnReleasedKeyNote(int keyIndex, int noteIndex)
        {
            _statisticEvents.Enqueue(new KeyStatisticEvent(false, keyIndex, noteIndex, (float)noteIndex/_maxNotes));
            _currentPressedKeys.Remove(keyIndex);
            _currentPressedNotes.Remove(noteIndex);
        }
        
        public void Update()
        {
            foreach (var statisticEvent in _statisticEvents)
            {
                statisticEvent.ElapsedTime += Time.deltaTime;
            }

            for (int i = 0; i < _statisticEvents.Count; i++)
            {
                var statisticEvent = _statisticEvents.Peek();
                if(statisticEvent.ElapsedTime <= MaxTimeSpan || _currentPressedKeys.Contains(statisticEvent.Key)) break;
                _statisticEvents.Dequeue();
            }

            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            int pressedOverTimeCount = 0;
            float averageTone = 0;
            float averageToneOverTime = 0;
            float lastTone = -1;
            float lastNoteLength = -1;
            float averageNoteLength = -1;
            float silenceLength = MaxTimeSpan;
            
            foreach (var statisticEvent in _statisticEvents)
            {
                if (_currentPressedNotes.Contains(statisticEvent.Note) && statisticEvent.Pressed)
                {
                    averageTone += statisticEvent.Tone;
                    lastTone = statisticEvent.Tone;
                    lastNoteLength = statisticEvent.ElapsedTime;
                    averageNoteLength += statisticEvent.ElapsedTime;
                }
                if (statisticEvent.Pressed)
                {
                    pressedOverTimeCount++;
                    averageToneOverTime += statisticEvent.Tone;
                }
                
                if(!statisticEvent.Pressed) 
                    silenceLength = statisticEvent.ElapsedTime;
            }

            if(_currentPressedNotes.Count >= 0)
            {
                averageNoteLength /= _currentPressedNotes.Count;
                averageTone /= _currentPressedNotes.Count;
            }
            if(pressedOverTimeCount > 0)
                averageToneOverTime /= pressedOverTimeCount;
            
            if (_currentPressedNotes.Count > 0)
                silenceLength = 0;
            
            _notePressedCount.Value = _currentPressedNotes.Count;
            _notePressCountOverTime.Value = pressedOverTimeCount;
            if(averageTone > 0)
                _averageTone.Value = averageTone;
            if(averageToneOverTime > 0)
                _averageToneOverTime.Value = averageToneOverTime;
            if(lastTone > 0)
                _lastTone.Value = lastTone;
            if(lastNoteLength > 0)
                _lastNoteLength.Value = lastNoteLength;
            if(averageNoteLength > 0)
                _averageNoteLength.Value = averageNoteLength;
            _silenceLength.Value = silenceLength;
        }
        
        private void UnsubscribeEvents()
        {
            _pianoKeyPresser.OnPressedKeyNoteIndexes -= OnPressedKeyNote;
            _pianoKeyPresser.OnReleasedKeyNoteIndexes -= OnReleasedKeyNote;
        }

        public void Dispose()
        {
            if(_pianoKeyPresser != null)
                UnsubscribeEvents();
        }
    }
}