using Game.Runtime.TerrainChunkSystem;
using Game.Runtime.WeatherSystem.WeatherTween;
using UnityEngine;

namespace Game.Runtime.PlanetSystem.Configs
{
    [CreateAssetMenu(fileName = "Biome", menuName = "Game/Biome")]
    public class BiomeConfig : ScriptableObject
    {
        public WeatherState WeatherState;
        public TerrainChunk TerrainPrefab;
    }
}