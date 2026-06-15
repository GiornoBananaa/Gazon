using System;

namespace Game.Runtime.MusicInstrumentSystem
{
    public readonly struct InstrumentId : IEquatable<InstrumentId>
    {
        public readonly MusicalInstrumentType Type;
        public readonly int Id;

        public InstrumentId(MusicalInstrumentType type, int id)
        {
            Type = type;
            Id = id;
        }

        public bool Equals(InstrumentId other)
        {
            return Type == other.Type && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is InstrumentId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Type, Id);
        }
    }
}