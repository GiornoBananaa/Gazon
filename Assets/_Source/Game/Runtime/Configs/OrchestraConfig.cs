using System;
using Game.Runtime.MusicInstrumentSystem;
using Game.Runtime.PianoFeature;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Runtime.Configs
{
    [CreateAssetMenu(fileName = "Orchestra", menuName = "Game/Orchestra")]
    public class OrchestraConfig : ScriptableObject
    {
        [Header("Positioning")]
        public float MaxArcAngle = 180;
        public float MinArcRadius = 3;
        
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