using Game.Runtime.CameraSystem;
using UnityEngine;

namespace Game.Runtime.CameraFeature
{
    public class PianoCameraState : ICameraState
    {
        private readonly CameraFollowTargetMover _mover;
        private readonly CameraInputRotator _rotator;
        private readonly Transform _targetTransform;
        private readonly Transform _forwardLimitYTransform;
        private readonly float _rotationLimitY;
        
        public PianoCameraState(CameraFollowTargetMover mover, CameraInputRotator rotator, Transform targetTransform, Transform forwardLimitYTransform, float rotationLimitY)
        {
            _mover = mover;
            _rotator = rotator;
            _targetTransform = targetTransform;
            _forwardLimitYTransform = forwardLimitYTransform;
            _rotationLimitY = rotationLimitY;
        }
        
        public void Enter()
        {
            _mover.Enable();
            _rotator.Enable();
            
            _mover.SetTarget(_targetTransform);
            _rotator.SetLimitY(_forwardLimitYTransform, _rotationLimitY);
            
        }

        public void Exit()
        {
            _mover.Disable();
            _rotator.Disable();
            
            _mover.SetTarget(null);
            _rotator.RemoveLimit();
        }
    }
}