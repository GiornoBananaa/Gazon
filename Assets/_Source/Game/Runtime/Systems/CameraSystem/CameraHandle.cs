using Game.Runtime.Configs;
using UnityEngine;

namespace Game.Runtime.CameraSystem
{
    public class CameraHandle : MonoBehaviour, ICurrentCamera
    {
        [SerializeField] private Camera _currentCamera;
        [field: SerializeField] public CameraConfig Config { get; private set; }
        [field: SerializeField] public Transform XRotationPivot { get; private set; }
        [field: SerializeField] public Transform YRotationPivot { get; private set; }

        public Camera GetCurrentCamera()
        {
            return _currentCamera;
        }
    }
}