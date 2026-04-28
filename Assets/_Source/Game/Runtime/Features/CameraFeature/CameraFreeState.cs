using Game.Runtime.CameraSystem;
using Game.Runtime.PlayerFeature;
using UnityEngine;

namespace Game.Runtime.CameraFeature
{
    public class CameraFreeState : ICameraState
    {
        private readonly CameraFollowTargetMover _mover;
        private readonly CameraInputRotator _rotator;
        private readonly Transform _targetTransform;
        
        public CameraFreeState(CameraFollowTargetMover mover, CameraInputRotator rotator, Player player)
        {
            _mover = mover;
            _rotator = rotator;
            _targetTransform = player.CameraPoint;
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