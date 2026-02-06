using Game.Runtime.Array2D;
using Game.Runtime.Array2D.Array2DTypes.ObjectTypes;
using Game.Runtime.Plugins.Array2D;
using UnityEngine;

namespace Game.Runtime.PlanetSystem.Configs
{
    [CreateAssetMenu(fileName = "Planet", menuName = "Game/Planet")]
    public class PlanetConfig : ScriptableObject
    {
        [Header("Biomes")]
        public float BiomeSize = 50;
        public float BiomeSizeInChunks = 4;
        public Array2DBiome Biomes;

        [Header("Weather")]
        public float ChunkWeatherBlendDistance = 5;
    }
}