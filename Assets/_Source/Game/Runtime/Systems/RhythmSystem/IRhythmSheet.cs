using System;
using System.Collections.Generic;
using Game.Runtime.MusicInstrumentSystem;

namespace Game.Runtime.RhythmSystem
{
    public interface IRhythmSheet
    {
        public IEnumerable<IEnumerable<RhythmKey>> RhythmKeys { get; }
        public float Time { get; }
        
        public event Action<RhythmKey, RhythmResult> OnRhythmResult;
        public event Action<RhythmKey, RhythmResult> OnRhythmEndResult;

        void SetInstrument(InstrumentId id, IInstrument instrument);
        void SetKeys(IEnumerable<IEnumerable<RhythmKey>> rhythmKeys);
        void StartSheet();
        void Clear();
        void StartKey(int id);
        void StopKey(int id);
    }
}