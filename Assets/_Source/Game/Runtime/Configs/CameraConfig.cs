using UnityEngine;

namespace Game.Runtime.Configs
{
    [CreateAssetMenu(fileName = "Camera", menuName = "Game/Camera")]
    public class CameraConfig : ScriptableObject
    {
        public float InputSensitivity = 1f;
        public float FollowSmoothTime = 1f;
    }
}