using UnityEngine;

namespace Game.Runtime.Utils
{
    public static class MathUtils
    {
        public static bool InRange(float target, float center, float range)
        {
            return Mathf.Abs(target - center) <= range;
        }
        
        public static Vector3 ConvertToRoundWorldPosition(Vector3 position, Vector3 cameraPosition)
        {
            Vector3 local = position - cameraPosition;
            float heightOffset = local.x * local.x * -Planet.GlobalConstants.ROUND_WORLD_VALUE + local.z * local.z * -Planet.GlobalConstants.ROUND_WORLD_VALUE;
            return new Vector3(position.x, position.y + heightOffset, position.z);
        }
    }
}