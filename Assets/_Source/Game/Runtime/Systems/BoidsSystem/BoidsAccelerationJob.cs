using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Runtime.BoidsSystem
{
    [BurstCompile]
    public struct BoidsAccelerationJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float3> Positions;
        [ReadOnly] public NativeArray<float3> Velocities;
        [ReadOnly] public NativeStream.Reader NearObjectReader;
        
        public NativeArray<float3> Accelerations;
        public float MaxSeparationDistance;
        
        public float SpreadWeight;
        public float VelocityWeight;
        public float PositionWeight;
        public float SeparationWeight;
    
        public void Execute(int index)
        {
            int count = NearObjectReader.BeginForEachIndex(index);
            
            float3 averageSpread = float3.zero;
            float3 averageVelocity = float3.zero;
            float3 averagePosition = float3.zero;
            float3 separationPosition = float3.zero;
            
            int separationCount = 0;
            int detectedCount = 0;
            for (int i = 0; i < count; i++)
            {
                int otherIndex = NearObjectReader.Read<int>();
                if(otherIndex == index)
                    continue;
                var targetPos = Positions[otherIndex];
                var posDifference = Positions[index] - targetPos;
                detectedCount++;
                averageSpread += math.normalize(posDifference);
                averageVelocity += Velocities[otherIndex];
                averagePosition += targetPos;
                
                if (math.lengthsq(posDifference) < MaxSeparationDistance * MaxSeparationDistance)
                {
                    separationPosition += targetPos;
                    separationCount++;
                }
            }
            
            NearObjectReader.EndForEachIndex();
            
            if (detectedCount <= 0) return;
            Accelerations[index] += (averageSpread / detectedCount) * SpreadWeight
                                    + (averageVelocity / detectedCount) * VelocityWeight
                                    + (averagePosition / detectedCount - Positions[index]) * PositionWeight;
            if (separationCount <= 0) return;
            Accelerations[index] += math.normalize(Positions[index] - separationPosition / separationCount) * SeparationWeight;
        }
    }
}