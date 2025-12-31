namespace Game.Runtime.Planet.Generation
{
    public interface IPlanetGenerator
    {
        TerrainChunk[,] Generate(Biome[,] biomes, float biomeSize, float chunkSize);
    }
}