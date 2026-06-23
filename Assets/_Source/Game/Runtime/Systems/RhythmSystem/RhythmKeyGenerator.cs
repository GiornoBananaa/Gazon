using System.Collections.Generic;
using System.Linq;
using Game.Runtime.MusicInstrumentSystem;
using Game.Runtime.Utils;
using UnityEngine;

namespace Game.Runtime.RhythmSystem
{
    public class RhythmKeyGenerator : IRhythmKeyGenerator
    {
        private readonly List<Note[]> _notes = new();
        private readonly List<Note> _notesMerged = new();
        private readonly List<InstrumentId> _instrumentIds = new();
        private readonly List<int> _indexes = new();
        private int _notesCount;
        
        public void AddNotes(InstrumentId id, IEnumerable<Note> notes)
        {
            var notesArray = notes.ToArray();
            _notesCount += notesArray.Length;
            _notes.Add(notesArray);
            _instrumentIds.Add(id);
            _indexes.Add(0);
            int index = 0;
            foreach (var note in notesArray)
            {
                for (int i = index; i <= _notesMerged.Count; i++)
                {
                    if(i >= _notesMerged.Count)
                    {
                        _notesMerged.Add(note);
                        index = i;
                        break;
                    }
                    if(note.StartTime < _notesMerged[i].StartTime)
                    {
                        _notesMerged.Insert(i, note);
                        index = i;
                        break;
                    }
                }
            }
        }

        public void Clear()
        {
            _notes.Clear();
            _notesMerged.Clear();
            _instrumentIds.Clear();
            _indexes.Clear();
            _notesCount = 0;
        }
        
        public List<RhythmKey>[] Generate(int keyCount, float maxNotesPerSecond)
        {
            List<RhythmKey>[] keys = new List<RhythmKey>[keyCount];
            for (int i = 0; i < keyCount; i++)
            {
                keys[i] = new List<RhythmKey>();
            }
            
            int iteratorStartIndex = 0;
            for (int i = 0; i < _notesCount; i++)
            {
                int instrumentIndex = 0;
                bool first = true;
                for (int j = 0; j < _notes.Count; j++)
                {
                    if (first && _indexes[j] < _notes[j].Length)
                    {
                        instrumentIndex = j;
                        first = false;
                        continue;
                    }
                    if(_indexes[j] < _notes[j].Length && _notes[j][_indexes[j]].StartTime < _notes[instrumentIndex][_indexes[instrumentIndex]].StartTime)
                        instrumentIndex = j;
                }
                
                var instrumentId = _instrumentIds[instrumentIndex];
                var note = _notes[instrumentIndex][_indexes[instrumentIndex]];
                
                float averageNumber = 0;
                int noteNearCount = 0;
                float min = float.MaxValue;
                float max = float.MinValue;
                bool firstNote = true;
                foreach (var noteInRange in GetNoteIndexesInRange(_notesMerged, note.StartTime, 0.8f, iteratorStartIndex))
                {
                    if(firstNote)
                    {
                        iteratorStartIndex = noteInRange;
                        firstNote = false;
                    }
                    averageNumber += _notesMerged[noteInRange].NoteNumber;
                    noteNearCount++;
                    
                    if (_notesMerged[noteInRange].NoteNumber < min) min = _notesMerged[noteInRange].NoteNumber;
                    if (_notesMerged[noteInRange].NoteNumber > max) max = _notesMerged[noteInRange].NoteNumber;
                }
                float numberCenter = averageNumber / noteNearCount;
                Vector2 spread = new Vector2(min, max);
                
                float centerAndSpreadDiff = numberCenter - (spread.y + spread.x) / 2;
            
                int key = Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp(0, keyCount - 1, Mathf.InverseLerp(spread.x + centerAndSpreadDiff, spread.y + centerAndSpreadDiff, note.NoteNumber))), 0, keyCount - 1);
            
                if(keys[key].Count > 0 && MathUtils.IsOverlapped(keys[key][^1].StartTime, keys[key][^1].EndTime, note.StartTime, note.EndTime))
                {
                    keys[key][^1].Notes.Add(note);
                    keys[key][^1].InstrumentIds.Add(instrumentId);
                }
                else
                    keys[key].Add(new RhythmKey(key, new List<Note> { note }, new List<InstrumentId> { instrumentId }));

                _indexes[instrumentIndex]++;
            }
            
            // squeeze note sequences
            foreach (var track in keys)
            {
                int[] minIndexes = new int[track.Count];
                for (int i = 1; i < track.Count; i++)
                {
                    int countOnTrack = 0;
                    int countAll = 0;
                    
                    for (int trackIndex = 0; trackIndex < keys.Length; trackIndex++)
                    {
                        for (int keyIndex = minIndexes[trackIndex]; keyIndex < keys[trackIndex].Count; keyIndex++)
                        {
                            if (keys[trackIndex][keyIndex].StartTime < track[i].StartTime - 0.4f)
                            {
                                minIndexes[trackIndex] = keyIndex + 1;
                                continue;
                            }
                            if (keys[trackIndex][keyIndex].StartTime > track[i].StartTime + 0.4f) break;
                            
                            if(track == keys[trackIndex])
                                countOnTrack++;
                            countAll++;
                        }
                    }

                    float maxNotes = (float)countOnTrack / countAll * maxNotesPerSecond;

                    if(track[i].StartTime - track[i - 1].StartTime > 1 / maxNotes) continue;
                    track[i - 1].Notes.AddRange(track[i].Notes);
                    track[i - 1].InstrumentIds.AddRange(track[i].InstrumentIds);
                    track.RemoveAt(i);
                    i--;
                }
            }
            
            // disconnect small and near notes
            int[] indexes = new int[keyCount];
            int[] minIndexesMerged = new int[keyCount];
            
            foreach (var note in _notesMerged)
            {
                for (int j = 0; j < indexes.Length; j++)
                {
                    if(indexes[j] >= keys[j].Count || !keys[j][indexes[j]].Notes.Contains(note)) continue;
                    
                    int smallNoteNearCount = 0;
                    bool passTime = false;
                    float lastTime = 0;
                    bool firstKey = true;
                    foreach (var noteInRange in GetKeyIndexesInRange(keys[j], keys[j][indexes[j]].StartTime, 0.8f, minIndexesMerged[j]))
                    {
                        if (firstKey)
                        {
                            firstKey = false;
                            minIndexesMerged[j] = noteInRange;
                        }
                        if (MathUtils.InRange(keys[j][indexes[j]].StartTime, keys[j][noteInRange].StartTime, keys[j][noteInRange].EndTime))
                            passTime = true;
                        if(keys[j][noteInRange].Length < 0.3f && keys[j][noteInRange].StartTime - lastTime < 0.25f)
                            smallNoteNearCount++;
                        else if (passTime) break;
                        else smallNoteNearCount = 0;
                    
                        lastTime = keys[j][noteInRange].EndTime;
                    }
                    
                    if (indexes[j] - 1 >= 0 && smallNoteNearCount > 2)
                    {
                        float diff = keys[j][indexes[j]].Notes[0].NoteNumber - keys[j][indexes[j] - 1].Notes[^1].NoteNumber;
                        
                        bool overlapped = false;
                        int newTrack = j;
                        if(diff > 0 && j + 1 < indexes.Length)
                        {
                            bool firstNearKey = true;
                            foreach (var nearKey in GetKeyIndexesInRange(keys[j + 1], keys[j][indexes[j]].StartTime, 0.8f, minIndexesMerged[j]))
                            {
                                if (firstNearKey)
                                {
                                    firstNearKey = false;
                                    minIndexesMerged[j + 1] = nearKey;
                                }
                                if (Mathf.Min(
                                        TimeSubtract(keys[j][indexes[j]].StartTime, keys[j + 1][nearKey].StartTime, keys[j + 1][nearKey].EndTime),
                                        TimeSubtract(keys[j][indexes[j]].EndTime, keys[j + 1][nearKey].StartTime, keys[j + 1][nearKey].EndTime)) <
                                    0.25f)
                                {
                                    overlapped = true;
                                    break;
                                }
                            }
                            if(!overlapped)
                                newTrack = j + 1;
                        }

                        if (diff < 0 && overlapped && j - 1 >= 0)
                        {
                            overlapped = false;
                            bool firstNearKey = true;
                            foreach (var nearKey in GetKeyIndexesInRange(keys[j - 1], keys[j][indexes[j]].StartTime, 0.8f, minIndexesMerged[j]))
                            {
                                if (firstNearKey)
                                {
                                    firstNearKey = false;
                                    minIndexesMerged[j - 1] = nearKey;
                                }
                                if (Mathf.Min(
                                        TimeSubtract(keys[j][indexes[j]].StartTime, keys[j - 1][nearKey].StartTime, keys[j - 1][nearKey].EndTime),
                                        TimeSubtract(keys[j][indexes[j]].EndTime, keys[j - 1][nearKey].StartTime, keys[j - 1][nearKey].EndTime)) < 0.25f)
                                {
                                    overlapped = true;
                                    break;
                                }
                            }
                            if(!overlapped)
                                newTrack = j - 1;
                        }

                        if (!overlapped)
                        {
                            keys[newTrack].Insert(indexes[newTrack], keys[j][indexes[j]]);
                            keys[j].RemoveAt(indexes[j]);
                            indexes[j]--;
                            indexes[newTrack]++;
                        }
                    }

                    indexes[j]++;
                    break;
                }
            }
            return keys;
        }
        
        private IEnumerable<int> GetNoteIndexesInRange(IList<Note> notes, float time, float radius, int startIndex = 0)
        {
            for (int i = startIndex; i < notes.Count; i++)
            {
                var note = notes[i];
                if(TimeSubtract(time, note.StartTime, note.EndTime) > radius)
                {
                    if(note.StartTime > time + radius) break;
                    continue;
                }
                yield return i;
            }
        }
        
        private IEnumerable<int> GetKeyIndexesInRange(IList<RhythmKey> notes, float time, float radius, int startIndex = 0)
        {
            for (int i = startIndex; i < notes.Count; i++)
            {
                var note = notes[i];
                if(TimeSubtract(time, note.StartTime, note.EndTime) > radius)
                {
                    if(note.StartTime > time + radius) break;
                    continue;
                }
                yield return i;
            }
        }
        
        private float TimeSubtract(float time, float start, float end)
        {
            if(time < start) return start - time;
            if(time > end) return time - end;
            return 0;
        }
    }
}