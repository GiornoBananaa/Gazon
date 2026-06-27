using Game.Runtime.CameraSystem;
using UnityEngine;

namespace Game.Runtime.CameraFeature
{
    public class CameraMenuState : ICameraState
    {
        private readonly CameraFollowTargetMover _mover;
        private readonly CameraInputRotator _rotator;
        private Transform _targetTransform;
        
        public CameraMenuState(CameraFollowTargetMover mover, CameraInputRotator rotator)
        {
            _mover = mover;
            _rotator = rotator;
        }

        public void SetTarget(Transform target)
        {
            _targetTransform = target;
        }
        
        public void Enter()
        {
            _mover.Enable();
            _rotator.Enable();
            
            _mover.SetTarget(_targetTransform);
            _rotator.SetLimit(_targetTransform, 0, 0);
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void Exit()
        {
            _mover.Disable();
            _rotator.Disable();
            
            _mover.SetTarget(null);
            _rotator.RemoveLimit();
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}