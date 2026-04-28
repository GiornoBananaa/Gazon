using Game.Runtime.InputFeature;
using Game.Runtime.PlayerFeature;
using Game.Runtime.PlayerInteractionSystem;
using Game.Runtime.StateMachineSystem;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class PianoPlayerInteraction : MonoBehaviour, IInteractable
    {
        [SerializeField] private Collider _interactableCollider;
        [SerializeField] private Transform _seatPoint;
        [SerializeField] private float _maxViewAngle = 90f;
        
        private GeneralNavigationInputListener _generalNavigationInputListener;
        private PianoInputListener _pianoInputListener;
        private IStateMachine<IPlayerState> _playerStateMachine;
        private IPlayerState _playerState;
        private IPlayerState _playerExitState;
        
        [Inject]
        public void Construct(PianoInputListener pianoInputListener,  GeneralNavigationInputListener generalNavigationInputListener,
            IStateMachine<IPlayerState> playerStateMachine, PlayerFreeWalkState playerFreeState, PlayerPianoSeatState playerPianoSeatState)
        {
            _generalNavigationInputListener = generalNavigationInputListener;
            _pianoInputListener = pianoInputListener;
            _playerStateMachine = playerStateMachine;
            
            _playerState = playerPianoSeatState;
            _playerExitState =  playerFreeState;
        }

        public void Interact()
        {
            TakeSeat();
        }

        private void TakeSeat()
        {
            _interactableCollider.enabled = false;
            _generalNavigationInputListener.Exit += GetUp;
            _playerStateMachine.ChangeState(_playerState);
            _pianoInputListener.EnableInput();
        }

        private void GetUp()
        {
            _interactableCollider.enabled = true;
            _generalNavigationInputListener.Exit -= GetUp;
            _playerStateMachine.ChangeState(_playerExitState);
            _pianoInputListener.DisableInput();
        }
    }
}