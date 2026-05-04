using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Runtime.SpatialHashing
{
    [BurstCompile]
    public struct FindInRadiusJob : IJob 
    {
        [ReadOnly] public NativeArray<float3> Positions;
        [ReadOnly] public NativeArray<HashAndIndex> HashAndIndices;
        public float3 Position;
        public float Radius;
        public float CellSize;
        [WriteOnly] public NativeList<int> ResultIndices;
            
        public void Execute()
        {
            float radiusSquared = Radius * Radius;
            int3 minGridPos = SpatialHashUtils.GridPosition(Position - Radius, CellSize);
            int3 maxGridPos = SpatialHashUtils.GridPosition(Position + Radius, CellSize);
                
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
                            float3 toParticle = Positions[particleIndex] - Position;

                            if (math.lengthsq(toParticle) <= radiusSquared) 
                            {
                                ResultIndices.Add(particleIndex);
                            }
                        }
                    }
                }
            }
        }
    }
}