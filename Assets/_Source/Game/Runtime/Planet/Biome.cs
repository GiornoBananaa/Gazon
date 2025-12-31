using Game.Runtime.Weather.WeatherTween;

namespace Game.Runtime.Planet
{
    public class Biome
    {
        public readonly WeatherState WeatherState;
        public readonly TerrainChunk TerrainPrefab;

        public Biome(TerrainChunk terrainPrefab, WeatherState weatherState)
        {
            TerrainPrefab = terrainPrefab;
            WeatherState = weatherState;
        }
    }
}