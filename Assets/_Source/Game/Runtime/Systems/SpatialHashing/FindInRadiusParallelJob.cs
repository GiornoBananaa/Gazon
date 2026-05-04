using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Runtime.SpatialHashing
{
    [BurstCompile]
    public struct FindInRadiusParallelJob : IJobParallelFor 
    {
        [ReadOnly] public NativeArray<float3> Positions;
        [ReadOnly] public NativeArray<HashAndIndex> HashAndIndices;
        public float Radius;
        public float CellSize;
        [WriteOnly] public NativeStream.Writer Writer;
            
        public void Execute(int index) 
        {
            Writer.BeginForEachIndex(index);
            float radiusSquared = Radius * Radius;
            var queryPosition = Positions[index];
            int3 minGridPos = SpatialHashUtils.GridPosition(queryPosition - Radius, CellSize);
            int3 maxGridPos = SpatialHashUtils.GridPosition(queryPosition + Radius, CellSize);
                
            for (int x = minGridPos.x; x <= maxGridPos.x; x++) 
            {
                for (int y = minGridPos.y; y <= maxGridPos.y; y++) 
                {
                    for (int z = minGridPos.z; z <= maxGridPos.z; z++)
                    {
                        int3 gridPos = new(x, y, z);
                        int hash = SpatialHashUtils.Hash(gridPos);
                            
                        int startIndex = SpatialHashUtils.BinarySearchFirst(HashAndIndices, hash);
                            
                        if (startIndex < 0) continue;
                            

                        for (int i = startIndex; i < HashAndIndices.Length && HashAndIndices[i].Hash == hash; i++) 
                        {
                            int particleIndex = HashAndIndices[i].Index;
                            float3 toParticle = Positions[particleIndex] - queryPosition;
                                
                            if (math.lengthsq(toParticle) <= radiusSquared) 
                            {
                                Writer.Write(particleIndex);
                            }
                        }
                    }
                }
            }
            Writer.EndForEachIndex();
        }
    }
}