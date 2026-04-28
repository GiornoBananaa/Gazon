using Game.Runtime.CameraFeature;
using Game.Runtime.CameraSystem;
using Game.Runtime.InputFeature;
using Game.Runtime.PlayerMovementSystem;
using Game.Runtime.StateMachineSystem;

namespace Game.Runtime.PlayerFeature
{
    public class PlayerFreeWalkState : IPlayerState
    {
        private readonly PlayerInputListener _playerInputListener;
        private readonly IStateMachine<ICameraState> _cameraStateMachine;
        private readonly IPlayerMovement _playerMovement;
        private readonly ICameraState _cameraState;
        
        public PlayerFreeWalkState(PlayerInputListener playerInputListener, 
            IStateMachine<ICameraState> cameraStateMachine, IPlayerMovement playerMovement, CameraFreeState cameraState)
        {
            _playerInputListener = playerInputListener;
            _cameraStateMachine = cameraStateMachine;
            _playerMovement = playerMovement;
            _cameraState = cameraState;
        }
        
        public void Enter()
        {
            _cameraStateMachine.ChangeState(_cameraState);
            _playerInputListener.EnableInput();
            _playerMovement.Enable();
        }

        public void Exit()
        {
            
        }
    }
}