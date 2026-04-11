using Game.Runtime.CameraSystem;
using UnityEngine;

namespace Game.Runtime.CameraFeature
{
    public class FreeCameraState : ICameraState
    {
        private readonly CameraFollowTargetMover _mover;
        private readonly CameraInputRotator _rotator;
        private readonly Transform _targetTransform;
        
        public FreeCameraState(CameraFollowTargetMover mover, CameraInputRotator rotator, Transform targetTransform)
        {
            _mover = mover;
            _rotator = rotator;
            _targetTransform = targetTransform;
        }
        
        public void Enter()
        {
            _mover.Enable();
            _rotator.Enable();
            
            _mover.SetTarget(_targetTransform);
            _rotator.RemoveLimit();
        }

        public void Exit()
        {
            _mover.Disable();
            _rotator.Disable();

            _mover.SetTarget(null);
        }
    }
}