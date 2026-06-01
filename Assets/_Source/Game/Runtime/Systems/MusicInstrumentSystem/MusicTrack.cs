using System.Collections.Generic;

namespace Game.Runtime.MusicInstrumentSystem
{
    public class MusicTrack
    {
        public readonly int Id;
        public readonly string Name;
        public readonly int Difficulty;
        public readonly Dictionary<MusicalInstrumentType, Note[]> Notes;

        public MusicTrack(int id, string name, int difficulty)
        {
            Id = id;
            Name = name;
            Difficulty = difficulty;
            Notes = new Dictionary<MusicalInstrumentType, Note[]>();
        }
    }
}