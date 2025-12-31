using Game.Runtime.Array2D.Array2DTypes.ObjectTypes;
using UnityEngine;

namespace Game.Runtime.Planet.Configs
{
    [CreateAssetMenu(fileName = "Planet", menuName = "Game/Planet")]
    public class PlanetConfig : ScriptableObject
    {
        public float BiomeSize = 50;
        public float BiomeSizeInChunks = 4;
        public Array2DBiome Biomes;

        public Biome[,] GetBiomes()
        {
            Biome[,] biomes = new Biome[Biomes.GridSize.x, Biomes.GridSize.y];

            for (int x = 0; x < Biomes.GridSize.x; x++)
            {
                for (int y = 0; y < Biomes.GridSize.y; y++)
                {
                    biomes[x, y] = new Biome(Biomes[x, y].TerrainPrefab, Biomes[x, y].WeatherState);
                }
            }
            
            return biomes;
        }
    }
}