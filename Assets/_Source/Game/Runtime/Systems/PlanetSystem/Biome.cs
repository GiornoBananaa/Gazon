namespace Game.Runtime.PlanetSystem
{
    public class Biome
    {
        //public readonly WeatherState WeatherState;
        public readonly TerrainChunk TerrainPrefab;

        public Biome(TerrainChunk terrainPrefab/*, WeatherState weatherState*/)
        {
            TerrainPrefab = terrainPrefab;
            //WeatherState = weatherState;
        }
    }
}