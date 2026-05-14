using System;
using System.Collections.Generic;
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
        public ReadOnlyReactiveProperty<int> BiomeSizeInChunks => _biomeSizeInChunks;

        private readonly ReactiveProperty<Biome> _currentBiome = new();
        private readonly ReactiveProperty<TerrainChunk> _currentChunk = new();
        private readonly ReactiveProperty<Vector2Int> _currentChunkIndex = new(new Vector2Int(-1, -1));
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

            ((IUpdatable)this).Update();
        }

        void IUpdatable.Update()
        {
            if (_chunks.CurrentValue == null || _chunks.CurrentValue.Length == 0) return;
            
            var camera = _currentCamera.GetCurrentCamera();
            if (camera == null) return;
            Vector3 cameraPosition = camera.transform.position;
            
            if (CurrentChunk.CurrentValue != null && IsOnChunk(cameraPosition, CurrentChunkIndex.CurrentValue))
                return;
            Vector2Int newChunk = FindChunk(cameraPosition);
            _currentChunkIndex.Value = newChunk;
            _currentBiome.Value = _biomes.CurrentValue[newChunk.x / _biomeSizeInChunks.CurrentValue,
                newChunk.y / _biomeSizeInChunks.CurrentValue];
            _currentChunk.Value = _chunks.CurrentValue[newChunk.x, newChunk.y];
        }
        
        public Vector2Int GetBiomeIndexByChunk(Vector2Int chunkIndex)
            => new(chunkIndex.x / _biomeSizeInChunks.CurrentValue, chunkIndex.y / _biomeSizeInChunks.CurrentValue);

        public Biome GetBiomeByChunk(Vector2Int chunkIndex)
            => Biomes.CurrentValue[chunkIndex.x / _biomeSizeInChunks.CurrentValue,
                chunkIndex.y / _biomeSizeInChunks.CurrentValue];

        public Vector3 GetChunkCenterPosition(Vector2Int chunkIndex)
        {
            return _chunks.CurrentValue[chunkIndex.x, chunkIndex.y].transform.position +
                   new Vector3(_chunkSize.CurrentValue / 2, 0, _chunkSize.CurrentValue / 2);
        }
        
        public Vector2Int FindChunk(Vector3 position)
        {
            Vector2Int chunk = Vector2Int.zero;
            bool foundChunk = false;
            float nearestSqrDistance = float.MaxValue;
            for (int i = 0; i < _chunks.CurrentValue.GetLength(0); i++)
            {
                for (int j = 0; j < _chunks.CurrentValue.GetLength(1); j++)
                {
                    Vector2Int index = new Vector2Int(i, j);

                    float sqrMagnitude = (position - GetChunkCenterPosition(index)).sqrMagnitude;
                    if (sqrMagnitude < nearestSqrDistance)
                    {
                        nearestSqrDistance = sqrMagnitude;
                        chunk = index;
                    }

                    if (!IsOnChunk(position, index)) continue;
                    chunk = index;
                    foundChunk = true;
                    break;
                }

                if (foundChunk)
                    break;
            }

            return chunk;
        }

        public bool IsOnChunk(Vector3 point, Vector2Int chunk)
        {
            Vector3 chunkPosition = GetChunkCenterPosition(chunk);
            return MathUtils.InRange(point.x, chunkPosition.x, _chunkSize.CurrentValue / 2)
                   && MathUtils.InRange(point.z, chunkPosition.z, _chunkSize.CurrentValue / 2);
        }

        public Vector2Int GetChunkClampedIndex(Vector2Int chunk)
        {
            return new Vector2Int(Mathf.RoundToInt(Mathf.Repeat(chunk.x, _chunks.CurrentValue.GetLength(0))),
                Mathf.RoundToInt(Mathf.Repeat(chunk.y, _chunks.CurrentValue.GetLength(1))));
        }
        
        public Vector2Int GetBiomeClampedIndex(Vector2Int biome)
        {
            return new Vector2Int(Mathf.RoundToInt(Mathf.Repeat(biome.x, _biomes.CurrentValue.GetLength(0))),
                Mathf.RoundToInt(Mathf.Repeat(biome.y, _biomes.CurrentValue.GetLength(1))));
        }
        
        public Vector2Int GetBiomeFromChunk(Vector2Int chunk)
        {
            return GetChunkClampedIndex(chunk) / _biomeSizeInChunks.CurrentValue;
        }
        
        public Vector2Int GetChunkRelativePosition(Vector2Int chunk, Vector2Int center)
        {
            int length = _biomeSizeInChunks.CurrentValue * _biomes.CurrentValue.GetLength(0);
            int min = -(length / 2 - length % 2);
            int max = length / 2;
            return new Vector2Int(Mathf.RoundToInt(MathUtils.Repeat(chunk.x - center.x, min, max)),
                Mathf.RoundToInt(MathUtils.Repeat(chunk.y - center.y, min, max)));
        }
    }
}