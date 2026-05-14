using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Runtime.BoidsSystem
{
    [BurstCompile]
    public struct BoidsGravityPointJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float3> Positions;
        [ReadOnly] public NativeArray<float3> GravityPoints;
        [ReadOnly] public NativeArray<float2> GravityPointsForce;
        [ReadOnly] public NativeArray<float2> GravityRadius;
        
        public NativeArray<float3> Accelerations;
    
        public void Execute(int index)
        {
            float3 gravityForce = float3.zero;
            
            for (int i = 0; i < GravityPoints.Length; i++)
            {
                var posDifference = GravityPoints[i] - Positions[index];
                float minRadiusSqr = GravityRadius[i].x * GravityRadius[i].x;
                float maxRadiusSqr = GravityRadius[i].y * GravityRadius[i].y;
                float distanceSqr = math.lengthsq(posDifference);
                if (distanceSqr > maxRadiusSqr || distanceSqr < minRadiusSqr) continue;
                float force = math.lerp(GravityPointsForce[i].x, GravityPointsForce[i].y, math.unlerp(maxRadiusSqr, minRadiusSqr, math.lengthsq(posDifference)));
                gravityForce += math.normalize(posDifference) * force;
            }
            Accelerations[index] += gravityForce;
        }
    }
}