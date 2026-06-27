using Game.Runtime.CameraSystem;
using Game.Runtime.PianoFeature;
using UnityEngine;

namespace Game.Runtime.CameraFeature
{
    public class CameraMusicInstrumentState : ICameraState
    {
        private readonly CameraFollowTargetMover _mover;
        private readonly CameraInputRotator _rotator;
        private readonly Transform _targetTransform;
        
        public CameraMusicInstrumentState(CameraFollowTargetMover mover, CameraInputRotator rotator, MusicInstrument musicInstrument)
        {
            _mover = mover;
            _rotator = rotator;
            _targetTransform = musicInstrument.SeatPoint;
        }
        
        public void Enter()
        {
            _mover.Enable();
            _rotator.Enable();
            
            _mover.SetTarget(_targetTransform);
            _rotator.SetLimit(_targetTransform, 120f, 120f);
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
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