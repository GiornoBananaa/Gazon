using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Runtime.Planet.Generation
{
    public class PlanetGenerator : IPlanetGenerator
    {
        private readonly Dictionary<Biome, ObjectPool<TerrainChunk>> _chunksPool = new();
        private TerrainChunk[,] _terrainChunks;

        public TerrainChunk[,] Generate(Biome[,] biomes, float biomeSize, float chunkSize)
        {
            if(_terrainChunks != null)
            {
                for (int i = 0; i < _terrainChunks.GetLength(0); i++)
                {
                    for (int j = 0; j < _terrainChunks.GetLength(1); j++)
                    {
                        TerrainChunk terrainChunk = _terrainChunks[i, j];
                        if (terrainChunk == null) continue;
                        GetBiomePool(biomes[i, j]).Release(terrainChunk);
                        terrainChunk.gameObject.SetActive(false);
                    }
                }
            }
            
            float biomeSizeInChunks = biomeSize / chunkSize;
            
            _terrainChunks = new TerrainChunk[(int)(biomes.GetLength(0) * biomeSizeInChunks), (int)(biomes.GetLength(1) * biomeSizeInChunks)];
            
            for (int i = 0; i < _terrainChunks.GetLength(0); i++)
            {
                for (int j = 0; j < _terrainChunks.GetLength(1); j++)
                {
                    TerrainChunk terrainChunk = GetBiomePool(biomes[(int)(i / biomeSizeInChunks), (int)(j / biomeSizeInChunks)]).Get();
                    terrainChunk.gameObject.SetActive(true);
                    terrainChunk.transform.position = new Vector3(i * chunkSize, 0, j * chunkSize);
                    terrainChunk.gameObject.name = $"TerrainChunk({i},{j})";
                    _terrainChunks[i ,j] = terrainChunk;
                }
            }

            return _terrainChunks;
        }

        private ObjectPool<TerrainChunk> GetBiomePool(Biome biome)
        {
            if(!_chunksPool.ContainsKey(biome))
                _chunksPool.Add(biome, new ObjectPool<TerrainChunk>(() => Object.Instantiate(biome.TerrainPrefab)));
            
            return _chunksPool[biome];
        }
    }
}