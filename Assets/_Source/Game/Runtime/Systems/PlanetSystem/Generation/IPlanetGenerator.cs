namespace Game.Runtime.PlanetSystem.Generation
{
    public interface IPlanetGenerator
    {
        TerrainChunk[,] Generate(Biome[,] biomes, float biomeSize, float chunkSize);
    }
}