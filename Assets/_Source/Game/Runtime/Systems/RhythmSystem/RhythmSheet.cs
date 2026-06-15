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
        private Dictionary<InstrumentId, IInstrument> _instruments;
        private List<RhythmKey>[] _mergedRhythmKeys;
        private int[] _indexes;
        private int[] _noteInRhythmIndexes;
        private bool[] _pressedKeys;
        private float _time;
        private int _tracksCount;
        private readonly float _mistakeThreshold = 0.4f;
        private readonly float _perfectTime = 0.08f;
        
        public IEnumerable<IEnumerable<RhythmKey>> RhythmKeys => _mergedRhythmKeys;
        public float Time => _time;

        public event Action<RhythmKey, RhythmResult> OnRhythmResult;
        public event Action<RhythmKey, RhythmResult> OnRhythmEndResult;
        
        public void SetKeys(List<RhythmKey>[] rhythmKeys, Dictionary<InstrumentId, IInstrument> instruments)
        {
            if(_instruments != null)
            {
                foreach (var instrument in _instruments.Values)
                {
                    instrument.NotesPlayer.OnTimeChanged -= OnTimeChanged;
                }
            }
            
            _instruments = instruments;
            _mergedRhythmKeys = rhythmKeys;
            _tracksCount = rhythmKeys.Length;
            _indexes = new int[_tracksCount];
            _noteInRhythmIndexes = new int[_tracksCount];
            _pressedKeys = new bool[_tracksCount];
            
            foreach (var track in rhythmKeys)
            {
                foreach (var rhythmKey in track)
                {
                    foreach (var note in rhythmKey.Notes)
                    {
                        instruments[note.InstrumentId].NotesPlayer.MuteNote(note);
                    }
                }
            }
            
            foreach (var instrument in _instruments.Values)
            {
                instrument.NotesPlayer.OnTimeChanged += OnTimeChanged;
            }
        }

        public void StartKey(int id)
        {
            if(_indexes[id] >= _mergedRhythmKeys[id].Count) return;
            RhythmKey rhythmKey = _mergedRhythmKeys[id][_indexes[id]];
            float startTime = rhythmKey.StartTime;
            var result = MathUtils.InRadius(_time, startTime, _mistakeThreshold) ? RhythmResult.Success : RhythmResult.Fail;
            _pressedKeys[id] = true;
            int firstNoteIndex = Mathf.Clamp(_noteInRhythmIndexes[id], 0, rhythmKey.Notes.Count - 1);
            float firstNoteTime = rhythmKey.Notes[firstNoteIndex].StartTime;
            
            for (int i = firstNoteIndex; i < rhythmKey.Notes.Count; i++)
            {
                var note = rhythmKey.Notes[i];
                float offset = note.StartTime - firstNoteTime;
                float time = note.StartTime;
                if(offset <= 0.3f)
                    time = Mathf.Lerp(_time + offset, note.StartTime, Mathf.InverseLerp(0, 0.3f, offset));
                _instruments[note.InstrumentId].NotesPlayer.UnmuteNote(note);
                _instruments[note.InstrumentId].NotesPlayer.ForceNoteChange(note, new Note(note.NoteNumber, note.Velocity, time, time + note.Length, note.InstrumentId));
            }
            OnRhythmResult?.Invoke(rhythmKey, result);
        }
        
        public void StopKey(int id)
        {
            if(_indexes[id] >= _mergedRhythmKeys[id].Count || !_pressedKeys[id]) return;
            RhythmKey rhythmKey = _mergedRhythmKeys[id][_indexes[id]];
            var result = rhythmKey.Notes[^1].StartTime - _time < 0 ? RhythmResult.Success : RhythmResult.Miss;
            _pressedKeys[id] = false;
            foreach (var note in rhythmKey.Notes)
            {
                _instruments[note.InstrumentId].NotesPlayer.ForceStopNote(note, true);
            }
            OnRhythmEndResult?.Invoke(rhythmKey, result);
        }
        
        private void OnTimeChanged(float time)
        {
            _time = time;
            for (int track = 0; track < _mergedRhythmKeys.Length; track++)
            {
                for (int i = _indexes[track]; i < _mergedRhythmKeys[track].Count; i++)
                {
                    float thresholdInRhythm = _mergedRhythmKeys[track][i].EndTime;
                    if (_noteInRhythmIndexes[track] == 0)
                        thresholdInRhythm =
                            Mathf.Max(thresholdInRhythm, _mergedRhythmKeys[track][i].StartTime + _perfectTime);
                    else if(_noteInRhythmIndexes[track] + 1 < _mergedRhythmKeys[track][i].Notes.Count)
                        thresholdInRhythm = Mathf.Lerp(_mergedRhythmKeys[track][i].Notes[_noteInRhythmIndexes[track]].EndTime, _mergedRhythmKeys[track][i].Notes[_noteInRhythmIndexes[track] + 1].StartTime, 0.5f);
                    
                    if (_noteInRhythmIndexes[track] < _mergedRhythmKeys[track][i].Notes.Count - 1 && _time > thresholdInRhythm)
                        _noteInRhythmIndexes[track]++;
                    
                    float threshold = _mergedRhythmKeys[track][i].EndTime;
                    if (i + 1 < _mergedRhythmKeys[track].Count)
                        threshold = Mathf.Lerp(_mergedRhythmKeys[track][i].EndTime, _mergedRhythmKeys[track][i + 1].StartTime, 0.5f);
                    if (_time < threshold) continue;
                    if(_pressedKeys[track])
                        OnRhythmEndResult?.Invoke(_mergedRhythmKeys[track][_indexes[track]], RhythmResult.Miss);
                    _indexes[track] = i + 1;
                    _noteInRhythmIndexes[track] = 0;
                    _pressedKeys[track] = false;
                }
            }
        }
        
        public void Dispose()
        {
            foreach (var instrument in _instruments.Values)
            {
                instrument.NotesPlayer.OnTimeChanged -= OnTimeChanged;
            }
        }
    }
}