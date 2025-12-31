using Game.Runtime.Weather.WeatherTween;
using UnityEngine;

namespace Game.Runtime.Planet.Configs
{
    [CreateAssetMenu(fileName = "Biome", menuName = "Game/Biome")]
    public class BiomeConfig : ScriptableObject
    {
        public WeatherState WeatherState;
        public TerrainChunk TerrainPrefab;
    }
}