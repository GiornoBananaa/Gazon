using Game.Runtime.CameraFeature;
using Game.Runtime.ServiceSystem;
using UnityEngine;

namespace Game.Runtime.CameraSystem
{
    public class CameraFollowTargetMover : ILateUpdatable
    {
        private readonly Transform _cameraPivot;
        private readonly float _smoothTime;
        private Transform _target;
        private Vector3 _velocity;
        private bool _enabled = true;
        
        public CameraFollowTargetMover(CameraHandle cameraHandle)
        {
            _cameraPivot = cameraHandle.transform;
            _smoothTime = cameraHandle.Config.FollowSmoothTime;
        }
        
        void ILateUpdatable.LateUpdate()
        {
            if(_target == null || !_enabled) return;
            
            _cameraPivot.position = Vector3.SmoothDamp(_cameraPivot.position, _target.position, ref _velocity, _smoothTime);
        }

        public void Enable()
        {
            _enabled = true;
        }
        
        public void Disable()
        {
            _enabled = false;
        }
        
        public void SetTarget(Transform target)
        {
            _target = target;
        }
    }
}