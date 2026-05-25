using System;
using System.Collections.Generic;
using Game.Runtime.MusicInstrumentSystem;
using Game.Runtime.Utils;
using UnityEngine;

namespace Game.Runtime.RhythmSystem
{
    public enum RhythmResult
    {
        Fail,
        Miss,
        Success,
    }
    
    public class RhythmSheet : IRhythmSheet, IDisposable
    {
        private readonly struct RhythmKeyResult
        {
            public readonly float TimeDifference;
            public readonly bool IsPressed;
            public readonly RhythmResult Type;

            public RhythmKeyResult(float timeDifference, bool isPressed, RhythmResult rhythmResultType)
            {
                TimeDifference = timeDifference;
                IsPressed = isPressed;
                Type = rhythmResultType;
            }
        }
        
        private readonly NotesPlayer _notesPlayer;
        private List<RhythmKey>[] _rhythmKeys;
        private List<RhythmKeyResult>[] _rhythmKeyResults;
        private int[] _indexes;
        private bool[] _pressedKeys;
        private float _time;
        private float _mistakeThreshold = 0.4f;
        
        public IEnumerable<IEnumerable<RhythmKey>> RhythmKeys => _rhythmKeys;
        public float Time => _time;

        public event Action<RhythmKey, RhythmResult> OnRhythmResult;
        public event Action<RhythmKey, RhythmResult> OnRhythmEndResult;
        
        public RhythmSheet(NotesPlayer notesPlayer)
        {
            _notesPlayer = notesPlayer;
            _notesPlayer.OnTimeChanged += OnTimeChanged;
        }
        
        public void SetKeys(List<RhythmKey>[] rhythmKeys)
        {
            _rhythmKeys = rhythmKeys;
            _rhythmKeyResults = new List<RhythmKeyResult>[rhythmKeys.Length];
            _indexes = new int[rhythmKeys.Length];
            _pressedKeys = new bool[rhythmKeys.Length];
            int i = 0;
            foreach (var track in rhythmKeys)
            {
                _rhythmKeyResults[i] = new List<RhythmKeyResult>();
                foreach (var rhythmKey in track)
                {
                    foreach (var note in rhythmKey.Notes)
                    {
                        _notesPlayer.MuteNote(note);
                    }
                }
                i++;
            }
        }

        public void StartKey(int id)
        {
            if(_indexes[id] >= _rhythmKeys[id].Count) return;
            RhythmKey rhythmKey = _rhythmKeys[id][_indexes[id]];
            float startTime = rhythmKey.StartTime;
            var result = MathUtils.InRadius(_time, startTime, _mistakeThreshold) ? RhythmResult.Success : RhythmResult.Fail;
            _rhythmKeyResults[id].Add(new RhythmKeyResult(_time, true, result));
            _pressedKeys[id] = true;
            if (result == RhythmResult.Success)
            {
                foreach (var note in rhythmKey.Notes)
                {
                    _notesPlayer.UnmuteNote(note);
                    _notesPlayer.ForceNoteChange(note, new Note(note.NoteNumber, _time, _time + rhythmKey.Length));
                }
            }
            OnRhythmResult?.Invoke(rhythmKey, result);
        }
        
        public void StopKey(int id)
        {
            if(_indexes[id] >= _rhythmKeys[id].Count) return;
            RhythmKey rhythmKey = _rhythmKeys[id][_indexes[id]];
            float endTime = rhythmKey.EndTime;
            var result = _time > endTime ? RhythmResult.Success : RhythmResult.Miss;
            _rhythmKeyResults[id].Add(new RhythmKeyResult(_time, true, RhythmResult.Success));
            _pressedKeys[id] = false;
            if (result == RhythmResult.Success)
            {
                foreach (var note in rhythmKey.Notes)
                {
                    _notesPlayer.ForceStopNote(note);
                }
            }
            OnRhythmEndResult?.Invoke(rhythmKey, result);
        }
        
        private void OnTimeChanged(float time)
        {
            _time = time;
            for (int track = 0; track < _rhythmKeys.Length; track++)
            {
                for (int i = _indexes[track]; i < _rhythmKeys[track].Count; i++)
                {
                    float threshold = _rhythmKeys[track][i].EndTime;
                    if (i + 1 < _rhythmKeys[track].Count)
                        threshold = Mathf.Lerp(_rhythmKeys[track][i].EndTime, _rhythmKeys[track][i + 1].StartTime, 0.5f);
                    if (_time < threshold) continue;
                    if(_pressedKeys[track])
                        OnRhythmEndResult?.Invoke(_rhythmKeys[track][_indexes[track]], RhythmResult.Miss);
                    _indexes[track] = i + 1;
                }
            }
        }
        
        public void Dispose()
        {
            _notesPlayer.OnTimeChanged -= OnTimeChanged;
        }
    }
}