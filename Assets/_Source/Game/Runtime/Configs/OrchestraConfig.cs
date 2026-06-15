using System;
using Game.Runtime.MusicInstrumentSystem;
using Game.Runtime.PianoFeature;
using UnityEngine;

namespace Game.Runtime.Configs
{
    [CreateAssetMenu(fileName = "Orchestra", menuName = "Game/Orchestra")]
    public class OrchestraConfig : ScriptableObject
    {
        public InstrumentConfig[] Instruments;
        
        [Header("Statistics")]
        public float StatisticTimeSpan = 1;
    }
    
    [Serializable]
    public class InstrumentConfig
    {
        public MusicalInstrumentType Type;
        public MusicInstrument Prefab;
    }
}