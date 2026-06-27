using Game.Runtime.CameraFeature;
using Game.Runtime.CameraSystem;
using Game.Runtime.InputFeature;
using Game.Runtime.PlayerMovementSystem;
using Game.Runtime.StateMachineSystem;
using UnityEngine;

namespace Game.Runtime.PlayerFeature
{
    public class PlayerMenuState : IPlayerState
    {
        private readonly PlayerInputListener _playerInputListener;
        private readonly IStateMachine<ICameraState> _cameraStateMachine;
        private readonly IPlayerMovement _playerMovement;
        private readonly CameraMenuState _cameraState;
        private Transform _transform;
        
        public PlayerMenuState(PlayerInputListener playerInputListener, IStateMachine<ICameraState> cameraStateMachine, 
            CameraMenuState cameraState, IPlayerMovement playerMovement)
        {
            _playerInputListener = playerInputListener;
            _cameraStateMachine = cameraStateMachine;
            _playerMovement = playerMovement;
            _cameraState = cameraState;
        }

        public void SetStandPoint(Transform transform)
        {
            _transform = transform;
        }
        
        public void Enter()
        {
            _cameraState.SetTarget(_transform);
            _cameraStateMachine.ChangeState(_cameraState);
            _playerInputListener.DisableInput();
            _playerMovement.Disable();
        }

        public void Exit()
        {
            
        }
    }
}