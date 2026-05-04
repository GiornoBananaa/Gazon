using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace Game.Runtime.BoidsSystem
{
    [BurstCompile]
    public struct BoidsMoveJob : IJobParallelForTransform
    {
        public NativeArray<float3> Positions;
        public NativeArray<float3> Velocities;
        public NativeArray<float3> Accelerations;
        public float DeltaTime;
        public float MaxVelocity;
        public float RotationSpeed;
    
        public void Execute(int index, TransformAccess transform)
        {
            var velocity = Velocities[index] + Accelerations[index] * DeltaTime;
            var direction = math.normalize(velocity);
            velocity = direction * math.clamp(math.length(velocity), 1,  MaxVelocity);
            transform.position += Vector3.RotateTowards(Velocities[index], velocity, RotationSpeed * DeltaTime, 0) * DeltaTime;
            transform.rotation = Quaternion.LookRotation(direction);
            
            Positions[index] = transform.position;
            Velocities[index] = velocity;
            Accelerations[index] = float3.zero;
        }
    }
}