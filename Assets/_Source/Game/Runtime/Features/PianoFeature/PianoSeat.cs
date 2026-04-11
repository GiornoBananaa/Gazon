using System;
using Game.Runtime.CameraFeature;
using Game.Runtime.CameraSystem;
using Game.Runtime.InputFeature;
using Game.Runtime.PianoRhythmSystem;
using Game.Runtime.PlayerFeature;
using Game.Runtime.PlayerInteractionSystem;
using Game.Runtime.StateMachineSystem;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class PianoSeat : MonoBehaviour, IInteractable
    {
        [SerializeField] private Collider _interactableCollider;
        [SerializeField] private Transform _seatPoint;
        [SerializeField] private Transform _audioPoint;
        [SerializeField] private float _maxViewAngle = 90f;
        [SerializeField] private float _minAudioDistance = 1f;
        [SerializeField] private float _maxAudioDistance = 20f;
        [SerializeField] private float _minVolume = 0.2f;

        private IStateMachine<ICameraState> _cameraStateMachine;
        private IStateMachine<IPianoState> _pianoStateMachine;
        private ICameraState _cameraState;
        private ICameraState _cameraExitState;
        private GeneralNavigationInputListener _generalNavigationInputListener;
        private PianoInputListener _pianoInputListener;
        private PlayerInputListener _playerInputListener;
        private FreePianoKeyPresser _freePianoKeyPresser;
        private IPianoNoteTweener _noteTweener;
        private CameraHandle _cameraHandle;
        
        [Inject]
        public void Construct(IStateMachine<ICameraState> cameraStateMachine, CameraFollowTargetMover mover, CameraInputRotator rotator, 
            GeneralNavigationInputListener generalNavigationInputListener, PianoInputListener pianoInputListener, 
            PlayerInputListener playerInputListener, FreePianoKeyPresser freePianoKeyPresser, IStateMachine<IPianoState> pianoStateMachine, Player player, 
            IPianoNoteTweener noteTweener, CameraHandle cameraHandle)
        {
            _cameraStateMachine = cameraStateMachine;
            _generalNavigationInputListener = generalNavigationInputListener;
            _pianoInputListener = pianoInputListener;
            _playerInputListener = playerInputListener;
            _freePianoKeyPresser = freePianoKeyPresser;
            _pianoStateMachine = pianoStateMachine;
            _noteTweener = noteTweener;
            _cameraHandle = cameraHandle;
            _cameraState = new PianoCameraState(mover, rotator, _seatPoint, _seatPoint, _maxViewAngle);
            _cameraExitState = new FreeCameraState(mover, rotator, player.CameraPoint);
        }

        private void Awake()
        {
            _pianoStateMachine.ChangeState(new FreePianoState(_freePianoKeyPresser));
            _pianoInputListener.DisableInput();
        }

        private void Update()
        {
            _noteTweener.SetPianoWorldPosition(_audioPoint.position);
            float distanceKoef = Mathf.InverseLerp(_maxAudioDistance, _minAudioDistance, Vector3.Distance(transform.position, _cameraHandle.transform.position));
            _noteTweener.SetSpatialBlend(distanceKoef);
            _noteTweener.SetMaxVolume(Mathf.Lerp(_minVolume, 1, distanceKoef));
        }

        public void Interact()
        {
            TakeSeat();
        }

        private void TakeSeat()
        {
            _cameraStateMachine.ChangeState(_cameraState);
            _pianoInputListener.EnableInput();
            _playerInputListener.DisableInput();
            _interactableCollider.enabled = false;
            _generalNavigationInputListener.Exit += GetUp;
        }

        private void GetUp()
        {
            _cameraStateMachine.ChangeState(_cameraExitState);
            _pianoInputListener.DisableInput();
            _playerInputListener.EnableInput();
            _interactableCollider.enabled = true;
            _generalNavigationInputListener.Exit -= GetUp;
        }
    }
}