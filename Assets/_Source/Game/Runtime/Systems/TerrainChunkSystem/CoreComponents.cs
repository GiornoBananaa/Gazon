namespace Game.Runtime.TerrainChunkSystem
{
    public interface IPlanetGenerator
    {
        TerrainChunk[,] Generate(Biome[,] biomes, float biomeSize, float chunkSize);
    }
    
    public interface IPlanetMover
    {
        void Update();
    }
    
    public class Biome
    {
        public readonly TerrainChunk TerrainPrefab;

        public Biome(TerrainChunk terrainPrefab)
        {
            TerrainPrefab = terrainPrefab;
        }
    }
}