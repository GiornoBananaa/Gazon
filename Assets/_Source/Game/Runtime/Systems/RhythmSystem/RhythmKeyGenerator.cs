using System.Collections.Generic;
using System.Linq;
using Game.Runtime.MusicInstrumentSystem;
using Game.Runtime.Utils;
using UnityEngine;

namespace Game.Runtime.RhythmSystem
{
    public class RhythmKeyGenerator : IRhythmKeyGenerator
    {
        public List<RhythmKey>[] Generate(Note[] notes, int keyCount, float maxNotesPerSecond)
        {
            //TODO: optimize key generation
            //TODO: set keys count per second as max speed
            List<RhythmKey>[] keys = new List<RhythmKey>[keyCount];
            for (int i = 0; i < keyCount; i++)
            {
                keys[i] = new List<RhythmKey>();
            }
            
            foreach (var note in notes)
            {
                float numberCenter = GetAverageNumber(notes, note.StartTime, 0.8f);
                Vector2 spread = GetNumberSpread(notes, note.StartTime, 0.8f);
                float centerAndSpreadDiff = numberCenter - (spread.y + spread.x) / 2;
                
                int key = Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp(0, keyCount - 1, Mathf.InverseLerp(spread.x + centerAndSpreadDiff, spread.y + centerAndSpreadDiff, note.NoteNumber))), 0, keyCount - 1);
                
                if(keys[key].Count > 0 && MathUtils.IsOverlapped(keys[key][^1].StartTime, keys[key][^1].EndTime, note.StartTime, note.EndTime))
                    keys[key][^1].Notes.Add(note);
                else
                    keys[key].Add(new RhythmKey(key, new List<Note> { note }));
            }
            
            // squeeze note sequences
            foreach (var track in keys)
            {
                for (int i = 1; i < track.Count; i++)
                {
                    int countOnTrack = 0;
                    int countAll = 0;

                    for (int trackIndex = 0; trackIndex < keys.Length; trackIndex++)
                    {
                        for (int keyIndex = 0; keyIndex < keys[trackIndex].Count; keyIndex++)
                        {
                            if (Mathf.Abs(track[i].StartTime - keys[trackIndex][keyIndex].StartTime) > 0.4f)
                            {
                                if (keys[trackIndex][keyIndex].StartTime > track[i].StartTime + 0.4f) break;
                                continue;
                            }
                            if(track == keys[trackIndex])
                                countOnTrack++;
                            countAll++;
                        }
                    }

                    float maxNotes = (float)countOnTrack / countAll * maxNotesPerSecond;

                    if(track[i].StartTime - track[i - 1].StartTime > 1 / maxNotes) continue;
                    track[i - 1].Notes.AddRange(track[i].Notes);
                    track.RemoveAt(i);
                    i--;
                }
            }
            
            // disconnect small and near notes
            int[] indexes = new int[keyCount];
            
            foreach (var note in notes)
            {
                for (int j = 0; j < indexes.Length; j++)
                {
                    if(indexes[j] >= keys[j].Count || !keys[j][indexes[j]].Notes.Contains(note)) continue;
                    if (indexes[j] - 1 >= 0 && IsSequenceOfSmall(keys[j], keys[j][indexes[j]].StartTime, 0.8f))
                    {
                        float diff = keys[j][indexes[j]].Notes[0].NoteNumber - keys[j][indexes[j] - 1].Notes[^1].NoteNumber;
                        
                        bool overlapped = false;
                        int newTrack = j;
                        if(diff > 0 && j + 1 < indexes.Length)
                        {
                            foreach (var nearKey in GetKeysInRange(keys[j + 1], keys[j][indexes[j]].StartTime, 0.8f))
                            {
                                if (Mathf.Min(
                                        TimeSubtract(keys[j][indexes[j]].StartTime, nearKey.StartTime, nearKey.EndTime),
                                        TimeSubtract(keys[j][indexes[j]].EndTime, nearKey.StartTime, nearKey.EndTime)) <
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
                            foreach (var nearKey in GetKeysInRange(keys[j - 1], keys[j][indexes[j]].StartTime, 0.8f))
                            {
                                if (Mathf.Min(
                                        TimeSubtract(keys[j][indexes[j]].StartTime, nearKey.StartTime, nearKey.EndTime),
                                        TimeSubtract(keys[j][indexes[j]].EndTime, nearKey.StartTime, nearKey.EndTime)) < 0.25f)
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
                }
            }
            
            return keys;
        }
        
        private bool IsSequenceOfSmall(IEnumerable<RhythmKey> notes, float time, float radius)
        {
            int smallNoteNearCount = 0;
            bool passTime = false;
            float lastTime = 0;
            foreach (var note in GetKeysInRange(notes, time, radius))
            {
                if (MathUtils.InRange(time, note.StartTime, note.EndTime))
                    passTime = true;
                if(note.Length < 0.3f && note.StartTime - lastTime < 0.25f)
                    smallNoteNearCount++;
                else if (passTime) break;
                else smallNoteNearCount = 0;
                    
                lastTime = note.EndTime;
            }
            return smallNoteNearCount > 2;
        }
        
        private float GetAverageNumber(IEnumerable<Note> notes, float time, float radius)
        {
            float averageNumber = 0;
            int noteNearCount = 0;
            foreach (var note in GetNotesInRange(notes, time, radius))
            {
                averageNumber += note.NoteNumber;
                noteNearCount++;
            }
            return averageNumber / noteNearCount;
        }
        
        private Vector2 GetNumberSpread(IEnumerable<Note> notes, float time, float radius)
        {
            float min = float.MaxValue;
            float max = float.MinValue;
            foreach (var note in GetNotesInRange(notes, time, radius))
            {
                if (note.NoteNumber < min) min = note.NoteNumber;
                if (note.NoteNumber > max) max = note.NoteNumber;
            }
            return new Vector2(min, max);
        }
        
        private float GetAveragePressedCountAtOnce(IEnumerable<Note> notes, float time, float radius)
        {
            int pressedInGroupCount = 0;
            int groupsCount = 0;
            float lastTime = 0;
            foreach (var note in GetNotesInRange(notes, time, radius))
            {
                pressedInGroupCount++;
                if (note.StartTime - lastTime > 0.1f)
                    groupsCount++;
                lastTime = note.StartTime;
            }
            return (float)pressedInGroupCount / groupsCount;
        }

        private int HandsNumber(Note[] notes, float time, float radius)
        {
            int lastNote = -1;
            int handsNumber = 0;
            int maxRadiusOfHand = 12;
            
            foreach (var note in GetNotesInRange(notes, time, radius).OrderBy(note => note.NoteNumber))
            {
                if (lastNote != -1 && note.NoteNumber - lastNote > maxRadiusOfHand)
                    handsNumber++;
                
                lastNote = note.NoteNumber;
            }
            
            return handsNumber;
        }
        
        private List<List<Note>> GetHandsGroup(IEnumerable<Note> notes, float time, float radius)
        {
            int lastNote = -1;
            float lastNoteStartTime = 0;
            int maxRadiusOfHand = 12;
            float maxTimeDiff = 0.2f;
            List<List<Note>> handsGroup = new List<List<Note>>();
            
            foreach (var note in GetNotesInRange(notes, time, radius).OrderBy(note => note.NoteNumber))
            {
                if (lastNote == -1 || note.NoteNumber - lastNote > maxRadiusOfHand && note.StartTime - lastNoteStartTime < maxTimeDiff)
                    handsGroup.Add(new List<Note> { note });
                else
                    handsGroup[^1].Add(note);
                
                lastNote = note.NoteNumber;
            }
            
            return handsGroup;
        }
        
        private IEnumerable<Note> GetNotesInRange(IEnumerable<Note> notes, float time, float radius)
        {
            foreach (var note in notes)
            {
                if(TimeSubtract(time, note.StartTime, note.EndTime) > radius)
                {
                    if(note.StartTime > time + radius) break;
                    continue;
                }
                yield return note;
            }
        }
        
        private IEnumerable<RhythmKey> GetKeysInRange(IEnumerable<RhythmKey> notes, float time, float radius)
        {
            foreach (var note in notes)
            {
                if(TimeSubtract(time, note.StartTime, note.EndTime) > radius)
                {
                    if(note.StartTime > time + radius) break;
                    continue;
                }
                yield return note;
            }
        }
        
        private int GetKeysCountInRange(IEnumerable<RhythmKey> notes, float time, float radius)
        {
            int count = 0;
            foreach (var note in notes)
            {
                if(TimeSubtract(time, note.StartTime, note.EndTime) > radius)
                {
                    if(note.StartTime > time + radius) break;
                    continue;
                }

                count++;
            }

            return count;
        }
        
        private float TimeSubtract(float time, float start, float end)
        {
            if(time < start) return start - time;
            if(time > end) return time - end;
            return 0;
        }
    }
}