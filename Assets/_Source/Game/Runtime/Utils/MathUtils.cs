using Game.Runtime.TerrainChunkSystem;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Runtime.Utils
{
    public static class MathUtils
    {
        public static bool InRadius(float target, float center, float radius)
        {
            return InRange(target, center - radius, center + radius);
        }
        
        public static bool InRange(float target, float start, float end)
        {
            return target >= start && target <= end;
        }
        
        public static Vector3 ConvertToRoundWorldPosition(Vector3 position, Vector3 cameraPosition, float roundWorldValue = GlobalConstants.ROUND_WORLD_VALUE)
        {
            Vector3 local = position - cameraPosition;
            float heightOffset = local.x * local.x * -roundWorldValue + local.z * local.z * -roundWorldValue;
            return new Vector3(position.x, position.y + heightOffset, position.z);
        }
        
        public static Vector2 GetVectorXY(this Vector3 position)
        {
            return new Vector2(position.x, position.z);
        }
        
        public static Vector3 GetVectorXZ(this Vector2 position, float y = 0)
        {
            return new Vector3(position.x, y, position.y);
        }
        
        public static float Repeat(float t, float min, float max)
        {
            return Mathf.Repeat(t - min, max - min) + min;
        }
        
        public static bool IsOverlapped(float start1, float end1, float start2, float end2)
        {
            return start1 < end2 && start2 < end1;
        }
        
        public static Vector2 PointOnCircle(float radius, float angle)
        {
            angle *= Mathf.Deg2Rad;
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);

            return new Vector2(x, y);
        }
        
        public static Quaternion ClampRotation(Quaternion q, Vector3 bounds)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
            angleX = Mathf.Clamp(angleX, -bounds.x, bounds.x);
            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);
            angleY = Mathf.Clamp(angleY, -bounds.y, bounds.y);
            q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

            float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
            angleZ = Mathf.Clamp(angleZ, -bounds.z, bounds.z);
            q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

            return q.normalized;
        }
    }
}