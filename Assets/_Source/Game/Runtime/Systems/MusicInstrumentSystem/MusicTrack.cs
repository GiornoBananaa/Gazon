using System;
using System.Collections.Generic;

namespace Game.Runtime.MusicInstrumentSystem
{
    public class MusicTrack
    {
        public readonly int Id;
        public readonly string Name;
        public readonly int Difficulty;
        public readonly Dictionary<InstrumentId, Note[]> Notes;
        public readonly MusicalInstrumentType[] InstrumentsInSheet;

        public MusicTrack(int id, string name, int difficulty, MusicalInstrumentType[] instrumentsInSheet)
        {
            Id = id;
            Name = name;
            Difficulty = difficulty;
            InstrumentsInSheet = instrumentsInSheet;
            Notes = new Dictionary<InstrumentId, Note[]>();
        }
    }
}