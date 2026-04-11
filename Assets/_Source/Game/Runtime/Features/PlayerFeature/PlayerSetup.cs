using Game.Runtime.CameraSystem;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.PlayerFeature
{
    public class PlayerSetup : MonoBehaviour
    {
        [SerializeField] private Transform _cameraPoint;

        [Inject]
        private void Construct(CameraFollowTargetMover cameraFollow)
        {
            cameraFollow.SetTarget(_cameraPoint);
            cameraFollow.Enable();
        }
    }
}