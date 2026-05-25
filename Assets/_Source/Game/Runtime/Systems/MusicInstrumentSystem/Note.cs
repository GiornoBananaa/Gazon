using System;

namespace Game.Runtime.MusicInstrumentSystem
{
    public struct Note : IEquatable<Note>
    {
        public int NoteNumber;
        public float StartTime;
        public float EndTime;

        public Note(int noteNumber, float startTime, float endTime)
        {
            NoteNumber = noteNumber;
            StartTime = startTime;
            EndTime = endTime;
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