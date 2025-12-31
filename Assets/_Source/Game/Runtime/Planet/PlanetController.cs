using Game.Runtime.Configs;
using Game.Runtime.Planet.Generation;
using Game.Runtime.Planet.Movement;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.Planet
{
    public class PlanetController : MonoBehaviour
    {
        [Inject] private readonly IPlanetGenerator _planetGenerator;
        [Inject] private readonly PlanetMap _planetMap;
        [Inject] private readonly PlanetMover _planetMover;
        
        private float _biomeSizeInChunks;
        private float _biomeSize;
        private float _chunkSize;
        private Biome[,] _biomes;
        private TerrainChunk[,] _chunks;

        private void Awake()
        {
            _biomes = RootConfig.Instance.PlanetConfig.GetBiomes();
            _biomeSize = RootConfig.Instance.PlanetConfig.BiomeSize;
            _biomeSizeInChunks = RootConfig.Instance.PlanetConfig.BiomeSizeInChunks;
            _chunkSize = _biomeSize / _biomeSizeInChunks;
            _chunks = _planetGenerator.Generate(_biomes, _biomeSize, _chunkSize);
            
            foreach (var chunk in _chunks)
            {
                chunk.transform.parent = transform;
            }
            
            _planetMap.SetMapData(_biomes, _chunks, _biomeSize, _chunkSize);
        }

        public void Update()
        {
            _planetMap.UpdatePlayerLocation();
            _planetMover.Update();
        }
    }
}