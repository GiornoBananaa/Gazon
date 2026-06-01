using System.Collections.Generic;
using Game.Runtime.MusicInstrumentSystem;

namespace Game.Runtime.RhythmSystem
{
    public interface IRhythmKeyGenerator
    {
        List<RhythmKey>[] Generate(Note[] notes, int keyCount, float maxNotesPerSecond);
    }
}