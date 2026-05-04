using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace Game.Runtime.BoidsSystem
{
    [BurstCompile]
    public struct BoidsBoundsJob : IJobParallelForTransform
    {
        public NativeArray<float3> Positions;
        public NativeArray<float3> Accelerations;
        public Vector3 AreaPosition;
        public Vector3 AreaSize;
        
        public void Execute(int index, TransformAccess transform)
        {
            float threshold = 0.1f;
            
            var pos = Positions[index];
            var size = AreaSize * 0.5f;

            if (pos.x > AreaPosition.x + size.x + threshold)
                transform.position = new float3(AreaPosition.x - size.x, pos.y, pos.z);
            else if (pos.x < AreaPosition.x - size.x)
                transform.position = new float3(AreaPosition.x + size.x, pos.y, pos.z);
            
            if (pos.z > AreaPosition.z + size.z + threshold)
                transform.position = new float3(pos.x, pos.y, AreaPosition.z - size.z);
            else if (pos.z < AreaPosition.z - size.z)
                transform.position = new float3(pos.x, pos.y, AreaPosition.z  + size.z);

            Vector3 forwardOnPlane = Vector3.ProjectOnPlane(transform.rotation * math.forward(), math.up()).normalized;
            
            if (pos.y > AreaPosition.y + size.y + threshold || pos.y < AreaPosition.y - size.y)
                Accelerations[index] += Compensate(AreaPosition.y - size.y - pos.y, math.up(), size.y)
                                        + Compensate(AreaPosition.y + size.y - pos.y, math.down(), size.y);
            else
                Accelerations[index] += Compensate(size.y - math.abs(AreaPosition.y - pos.y), forwardOnPlane, size.y);
            
            Positions[index] = transform.position;
        }
        
        private float3 Compensate(float delta, float3 direction, float threshold)
        {
            const float multiplier = 100f;
            delta = math.abs(delta);
            return direction * (1 - delta / threshold) * multiplier;
        }
    }
}