using Game.Runtime.CameraSystem;
using Game.Runtime.PianoFeature;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Runtime.CameraFeature
{
    public class CameraPianoState : ICameraState
    {
        private readonly CameraFollowTargetMover _mover;
        private readonly CameraInputRotator _rotator;
        private readonly Transform _targetTransform;
        private readonly float _rotationLimitY;
        
        public CameraPianoState(CameraFollowTargetMover mover, CameraInputRotator rotator, MusicInstrument musicInstrument)
        {
            _mover = mover;
            _rotator = rotator;
            _targetTransform = musicInstrument.SeatPoint;
            _rotationLimitY = musicInstrument.MaxViewAngle;
        }
        
        public void Enter()
        {
            _mover.Enable();
            _rotator.Enable();
            
            _mover.SetTarget(_targetTransform);
            _rotator.SetLimitY(_targetTransform, 0, 0);
            
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