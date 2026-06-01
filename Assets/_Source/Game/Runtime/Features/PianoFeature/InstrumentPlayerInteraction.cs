using Game.Runtime.InputFeature;
using Game.Runtime.PlayerFeature;
using Game.Runtime.PlayerInteractionSystem;
using Game.Runtime.StateMachineSystem;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class InstrumentPlayerInteraction : MonoBehaviour, IInteractable
    {
        [SerializeField] private Collider _interactableCollider;
        [SerializeField] private Transform _seatPoint;
        [SerializeField] private float _maxViewAngle = 90f;
        
        private GeneralNavigationInputListener _generalNavigationInputListener;
        private IStateMachine<IPlayerState> _playerStateMachine;
        private IPlayerState _playerState;
        private IPlayerState _playerExitState;
        
        private IStateMachine<IInstrumentState> _pianoStateMachine;
        private IInstrumentState _instrumentState;
        
        [Inject]
        public void Construct(GeneralNavigationInputListener generalNavigationInputListener,
            IStateMachine<IPlayerState> playerStateMachine, PlayerFreeWalkState playerFreeState, 
            PlayerInstrumentSeatState playerInstrumentSeatState, IStateMachine<IInstrumentState> pianoStateMachine, FreeInstrumentState freeInstrumentState)
        {
            _generalNavigationInputListener = generalNavigationInputListener;
            _playerStateMachine = playerStateMachine;
            
            _playerState = playerInstrumentSeatState;
            _playerExitState =  playerFreeState;
            _pianoStateMachine = pianoStateMachine;
            _instrumentState = freeInstrumentState;
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
            _pianoStateMachine.ChangeState(_instrumentState);
        }

        private void GetUp()
        {
            _interactableCollider.enabled = true;
            _generalNavigationInputListener.Exit -= GetUp;
            _playerStateMachine.ChangeState(_playerExitState);
            _pianoStateMachine.ChangeState(null);
        }
    }
}