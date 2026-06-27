using System.Collections.Generic;
using Game.Runtime.ScenarioSystem;

namespace Game.Runtime.MusicInstrumentSystem
{
    public class MusicTrack
    {
        public readonly int Id;
        public readonly string Name;
        public readonly int Difficulty;
        public readonly Dictionary<InstrumentId, Note[]> Notes;
        public readonly MusicalInstrumentType[] InstrumentsInSheet;
        public readonly Scenario Scenario;
        public readonly string Path;

        public MusicTrack(int id, string name, int difficulty, MusicalInstrumentType[] instrumentsInSheet, Scenario scenario, string path)
        {
            Id = id;
            Name = name;
            Difficulty = difficulty;
            InstrumentsInSheet = instrumentsInSheet;
            Scenario = scenario;
            Path = path;
            Notes = new Dictionary<InstrumentId, Note[]>();
        }

        public float GetLength()
        {
            float max = 0;
            foreach (var notes in Notes.Values)
            {
                if(notes[^1].EndTime > max)
                    max = notes[^1].EndTime;
            }
            return max;
        }
    }
}