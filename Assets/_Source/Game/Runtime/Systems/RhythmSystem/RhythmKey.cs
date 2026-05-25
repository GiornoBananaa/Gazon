using System;
using System.Collections.Generic;
using Game.Runtime.MusicInstrumentSystem;

namespace Game.Runtime.RhythmSystem
{
    public readonly struct RhythmKey : IEquatable<RhythmKey>
    {
        public readonly int KeyIndex;
        public readonly float StartTime;
        public readonly float EndTime;
        public readonly List<Note> Notes;

        public RhythmKey(int keyIndex, List<Note> notes)
        {
            KeyIndex = keyIndex;
            StartTime = notes[0].StartTime;
            EndTime = notes[^1].EndTime;
            Notes = notes;
        }

        public float Length => EndTime - StartTime;

        public bool Equals(RhythmKey other)
        {
            return KeyIndex == other.KeyIndex && StartTime.Equals(other.StartTime) && EndTime.Equals(other.EndTime);
        }

        public override bool Equals(object obj)
        {
            return obj is RhythmKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(KeyIndex, StartTime, EndTime);
        }
    }
}