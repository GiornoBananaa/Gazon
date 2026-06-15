using System.Collections.Generic;
using Game.Runtime.MusicInstrumentSystem;

namespace Game.Runtime.RhythmSystem
{
    public interface IRhythmKeyGenerator
    {
        void AddNotes(InstrumentId id, IEnumerable<Note> notes);
        public void Clear();
        List<RhythmKey>[] Generate(int keyCount, float maxNotesPerSecond);
    }
}