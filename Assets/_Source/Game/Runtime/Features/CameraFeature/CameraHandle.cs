using Game.Runtime.Configs;
using UnityEngine;

namespace Game.Runtime.CameraFeature
{
    public class CameraHandle : MonoBehaviour
    {
        [field: SerializeField] public CameraConfig Config { get; private set; }
        [field: SerializeField] public Transform XRotationPivot { get; private set; }
        [field: SerializeField] public Transform YRotationPivot { get; private set; }
    }
}