using System;
using Game.Runtime.Configs;

namespace Game.Runtime.MusicInstrumentSystem
{
    public struct Note : IEquatable<Note>
    {
        public readonly int NoteNumber;
        public readonly NoteType NoteType;
        public readonly int Octave;
        public readonly float Velocity;
        public readonly float StartTime;
        public readonly float EndTime;
        public InstrumentId InstrumentId;

        public Note(int noteNumber, float velocity, float startTime, float endTime, InstrumentId instrumentId = default)
        {
            NoteNumber = noteNumber;
            NoteType = (NoteType)(noteNumber % 12);
            Octave = (noteNumber / 12) - 1;
            Velocity = velocity;
            StartTime = startTime;
            EndTime = endTime;
            InstrumentId = instrumentId;
        }

        public float Length => EndTime - StartTime;

        public static bool operator ==(Note c1, Note c2) 
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(Note c1, Note c2) 
        {
            return !c1.Equals(c2);
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(NoteNumber, StartTime);
        }

        public override bool Equals(object obj)
        {
            return obj is Note other && Equals(other);
        }
        
        public bool Equals(Note other)
        {
            return GetHashCode() == other.GetHashCode();
        }
    }
}