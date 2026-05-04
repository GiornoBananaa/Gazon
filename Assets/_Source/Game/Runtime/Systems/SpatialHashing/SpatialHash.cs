using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace Game.Runtime.SpatialHashing
{
    public class SpatialHash<T> : IDisposable where T : Component
    {
        private readonly float _cellSize;

        private readonly T[] _particleInstances;
        
        private TransformAccessArray _transformAccessArray;
        private NativeArray<float3> _positions;
        private NativeArray<HashAndIndex> _hashAndIndices;

        public SpatialHash(IEnumerable<T> instances, float cellSize)
        {
            _particleInstances = instances as T[] ?? instances.ToArray();
            _cellSize = cellSize;
            
            _positions = new NativeArray<float3>(_particleInstances.Length, Allocator.Persistent);
            _hashAndIndices = new NativeArray<HashAndIndex>(_particleInstances.Length, Allocator.Persistent);

            _transformAccessArray = new TransformAccessArray(_particleInstances.Select(i => i.transform).ToArray());
        }
        
        public JobHandle UpdateJob(JobHandle dependOn = default) 
        {
            var hashJob = new HashParticlesJob 
            {
                Positions = _positions,
                CellSize = _cellSize,
                HashAndIndices = _hashAndIndices
            };
            
            JobHandle hashJobHandle = hashJob.Schedule(_transformAccessArray, dependOn);
            
            var sortJob = new SortHashCodesJob 
            {
                HashAndIndices = _hashAndIndices
            };
            
            JobHandle sortJobHandle = sortJob.Schedule(hashJobHandle);
            
            return sortJobHandle;
        }

        public T[] FindAllInRadius(Vector3 position, float radius)
        {
            var list = new NativeList<int>(Allocator.TempJob);
            FindAllInRadius(position, radius, list);
            
            T[] result = new T[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                result[i] = _particleInstances[list[i]];
            }
            list.Dispose();
            
            return result;
        }
        
        public void FindAllInRadius(Vector3 position, float radius, NativeList<int> list)
        {
            var queryJob = new FindInRadiusJob 
            {
                Positions = _positions,
                HashAndIndices = _hashAndIndices,
                Position = position,
                Radius = radius,
                CellSize = _cellSize,
                ResultIndices = list
            };
            
            JobHandle queryJobHandle = queryJob.Schedule();
            queryJobHandle.Complete();
        }
        
        public JobHandle FindAllInRadiusParallel(float radius, NativeStream stream, int count, int batchSize, JobHandle dependsOn = default)
        {
            var queryJob = new FindInRadiusParallelJob
            {
                Positions = _positions,
                HashAndIndices = _hashAndIndices,
                Radius = radius,
                CellSize = _cellSize,
                Writer = stream.AsWriter()
            };
            
            var queryJobHandle = queryJob.Schedule(count, batchSize, dependsOn);
            return queryJobHandle;
        }
        
        public void Dispose() 
        {
            if (_transformAccessArray.isCreated) _transformAccessArray.Dispose();
            if (_positions.IsCreated) _positions.Dispose();
            if (_hashAndIndices.IsCreated) _hashAndIndices.Dispose();
        }
    }
}