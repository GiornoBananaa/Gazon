using Game.Runtime.TerrainChunkSystem;
using UnityEngine;

namespace Game.Runtime.Utils
{
    public static class MathUtils
    {
        public static bool InRange(float target, float center, float range)
        {
            return Mathf.Abs(target - center) <= range;
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
        
        public static Vector3 GetVectorXZ(this Vector2 position)
        {
            return new Vector3(position.x, 0, position.y);
        }
        
        public static float Repeat(float t, float min, float max)
        {
            return Mathf.Repeat(t - min, max - min) + min;
        }
    }
}