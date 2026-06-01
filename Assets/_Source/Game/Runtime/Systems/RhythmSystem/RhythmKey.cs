using System;
using System.Collections.Generic;
using Game.Runtime.MusicInstrumentSystem;

namespace Game.Runtime.RhythmSystem
{
    public readonly struct RhythmKey : IEquatable<RhythmKey>
    {
        public readonly int KeyIndex;
        public readonly List<Note> Notes;

        public RhythmKey(int keyIndex, List<Note> notes)
        {
            KeyIndex = keyIndex;
            Notes = notes;
        }

        public float StartTime => Notes[0].StartTime;
        public float EndTime => Notes[^1].EndTime;
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