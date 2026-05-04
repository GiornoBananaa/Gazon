using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;

namespace Game.Runtime.SpatialHashing
{
    [BurstCompile]
    public struct HashParticlesJob : IJobParallelForTransform
    {
        [WriteOnly] public NativeArray<float3> Positions;
        [WriteOnly] public NativeArray<HashAndIndex> HashAndIndices;
        public float CellSize;
            
        public void Execute(int index, TransformAccess transformAccess) 
        {
            int hash = SpatialHashUtils.Hash(SpatialHashUtils.GridPosition(transformAccess.position, CellSize));
                
            HashAndIndices[index] = new HashAndIndex { Hash = hash, Index = index };
            Positions[index] = transformAccess.position;
        }
    }
}