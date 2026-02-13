using Game.Runtime.CameraSystem;
using Game.Runtime.ServiceSystem;
using Game.Runtime.Utils;
using R3;
using UnityEngine;

namespace Game.Runtime.TerrainChunkSystem
{
    public class PlanetMap : IUpdatable
    {
        public ReadOnlyReactiveProperty<Biome> CurrentBiome => _currentBiome;
        public ReadOnlyReactiveProperty<TerrainChunk> CurrentChunk => _currentChunk;
        public ReadOnlyReactiveProperty<Vector2Int> CurrentChunkIndex => _currentChunkIndex;
        public ReadOnlyReactiveProperty<TerrainChunk[,]> Chunks => _chunks;
        public ReadOnlyReactiveProperty<Biome[,]> Biomes => _biomes;
        public ReadOnlyReactiveProperty<float> ChunkSize => _chunkSize;
        public ReadOnlyReactiveProperty<float> BiomeSize => _biomeSize;
        
        private readonly ReactiveProperty<Biome> _currentBiome = new();
        private readonly ReactiveProperty<TerrainChunk> _currentChunk = new();
        private readonly ReactiveProperty<Vector2Int> _currentChunkIndex = new();
        private readonly ReactiveProperty<TerrainChunk[,]> _chunks = new();
        private readonly ReactiveProperty<Biome[,]> _biomes = new();
        private readonly ReactiveProperty<float> _biomeSize = new();
        private readonly ReactiveProperty<float> _chunkSize = new();
        private readonly ReactiveProperty<int> _biomeSizeInChunks = new();
        
        private readonly ICurrentCamera _currentCamera;
        
        public PlanetMap(ICurrentCamera currentCamera)
        {
            _currentCamera = currentCamera;
        }

        public void SetMapData(Biome[,] biomes, TerrainChunk[,] chunks, float biomeSize, float chunkSize)
        {
            _biomes.Value = biomes;
            _chunks.Value = chunks;
            
            _biomeSize.Value = biomeSize;
            _chunkSize.Value = chunkSize;
            _biomeSizeInChunks.Value = (int)(biomeSize / chunkSize);
            
            _currentBiome.Value = null;
            _currentChunk.Value = null;
            
            Update();
        }

        public void Update()
        {
            if(_chunks.CurrentValue == null || _chunks.CurrentValue.Length == 0) return;
            
            Vector3 cameraPosition = _currentCamera.GetCurrentCamera().transform.position;
            
            if (CurrentChunk.CurrentValue != null && IsOnChunk(cameraPosition, CurrentChunk.CurrentValue.transform.position))
                return;
            
            Vector2Int newChunk = Vector2Int.zero;
            for (int i = 0; i < _chunks.CurrentValue.GetLength(0); i++)
            {
                for (int j = 0; j < _chunks.CurrentValue.GetLength(1); j++)
                {
                    if(!IsOnChunk(cameraPosition, _chunks.CurrentValue[i, j].transform.position)) continue;
                    newChunk = new Vector2Int(i, j);
                    break;
                }
            }
            
            _currentChunkIndex.Value = newChunk;
            _currentBiome.Value = _biomes.CurrentValue[newChunk.x / _biomeSizeInChunks.CurrentValue, newChunk.y / _biomeSizeInChunks.CurrentValue];
            _currentChunk.Value = _chunks.CurrentValue[newChunk.x, newChunk.y];
        }

        public Vector2Int GetBiomeIndexByChunk(Vector2Int chunkIndex) 
            => new(chunkIndex.x / _biomeSizeInChunks.CurrentValue, chunkIndex.y / _biomeSizeInChunks.CurrentValue);
        
        public Biome GetBiomeByChunk(Vector2Int chunkIndex) 
            => Biomes.CurrentValue[chunkIndex.x / _biomeSizeInChunks.CurrentValue, chunkIndex.y / _biomeSizeInChunks.CurrentValue];
        
        public Vector3 GetChunkCenterPosition(Vector2Int chunkIndex)
        {
            return _chunks.CurrentValue[chunkIndex.x, chunkIndex.y].transform.position +
            new Vector3(_chunkSize.CurrentValue / 2, 0, _chunkSize.CurrentValue / 2);
        }
        
        private bool IsOnChunk(Vector3 point, Vector3 chunkPosition)
        {
            return MathUtils.InRange(point.x, chunkPosition.x + _chunkSize.CurrentValue / 2, _chunkSize.CurrentValue / 2)
                   && MathUtils.InRange(point.z, chunkPosition.z + _chunkSize.CurrentValue / 2, _chunkSize.CurrentValue / 2);
        }
    }
}