using System;
using System.Collections.Generic;

namespace Game.Runtime.RhythmSystem
{
    public interface IRhythmSheet
    {
        public IEnumerable<IEnumerable<RhythmKey>> RhythmKeys { get; }
        public float Time { get; }
        
        public event Action<RhythmKey, RhythmResult> OnRhythmResult;
        public event Action<RhythmKey, RhythmResult> OnRhythmEndResult;
        
        void SetKeys(List<RhythmKey>[] rhythmKeys);
        void StartKey(int id);
        void StopKey(int id);
    }
}