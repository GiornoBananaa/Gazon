using System.Collections.Generic;
using Game.Runtime.Array2D.Array2DTypes.ObjectTypes;
using UnityEngine;

namespace Game.Runtime.Planet.Configs
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
        
        public Biome[,] GetBiomes()
        {
            Biome[,] biomes = new Biome[Biomes.GridSize.x, Biomes.GridSize.y];

            Dictionary<BiomeConfig, Biome> biomeInstances = new Dictionary<BiomeConfig, Biome>();
            
            for (int x = 0; x < Biomes.GridSize.x; x++)
            {
                for (int y = 0; y < Biomes.GridSize.y; y++)
                {
                    if (!biomeInstances.ContainsKey(Biomes[x, y]))
                        biomeInstances.Add(Biomes[x, y], new Biome(Biomes[x, y].TerrainPrefab, Biomes[x, y].WeatherState));
                        
                    biomes[x, y] = biomeInstances[Biomes[x, y]];
                }
            }
            
            return biomes;
        }
    }
}