using Game.Runtime.CameraSystem;
using Game.Runtime.PianoFeature;
using UnityEngine;

namespace Game.Runtime.CameraFeature
{
    public class CameraPianoState : ICameraState
    {
        private readonly CameraFollowTargetMover _mover;
        private readonly CameraInputRotator _rotator;
        private readonly Transform _targetTransform;
        private readonly float _rotationLimitY;
        
        public CameraPianoState(CameraFollowTargetMover mover, CameraInputRotator rotator, Piano piano)
        {
            _mover = mover;
            _rotator = rotator;
            _targetTransform = piano.SeatPoint;
            _rotationLimitY = piano.MaxViewAngle;
        }
        
        public void Enter()
        {
            _mover.Enable();
            _rotator.Enable();
            
            _mover.SetTarget(_targetTransform);
            _rotator.SetLimitY(_targetTransform, _rotationLimitY);
            
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