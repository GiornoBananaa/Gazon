using System.Linq;
using DG.Tweening;
using Game.Runtime.CameraSystem;
using Game.Runtime.MusicInstrumentSystem;
using Game.Runtime.PlayerFeature;
using Game.Runtime.PlayerInteractionSystem;
using Game.Runtime.StateMachineSystem;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public interface IInstrumentMenu
    {
        void Show();
        void Hide();
    }
    
    public class PianoModeMenu : MonoBehaviour, IInstrumentMenu
    {
        [SerializeField] private RhythmPianoControlPanelView _rhythmPianoControlPanelView;
        [SerializeField] private RotationSwitch _rhythmModeLever;

        private IStateMachine<IPlayerState> _playerStateMachine;
        private PlayerMenuState _playerMenuState;
        private PlayerInstrumentSeatState _playerInstrumentSeatState;
        private MusicInstrument _musicInstrument;
        private IStateMachine<IInstrumentState> _stateMachine;
        private RhythmInstrumentState _rhythmInstrumentState;
        private CameraInputRotator _cameraInputRotator;
        
        private float _defaultHeight;
        private float _hideHeight;
        private Tween _tween;
        
        [Inject]
        public void Construct(IStateMachine<IPlayerState> playerStateMachine, PlayerMenuState playerMenuState, PlayerInstrumentSeatState playerInstrumentSeatState, 
            MusicInstrument musicInstrument, IStateMachine<IInstrumentState> stateMachine, RhythmInstrumentState rhythmInstrumentState, CameraInputRotator cameraInputRotator)
        {
            _playerStateMachine = playerStateMachine;
            _playerMenuState = playerMenuState;
            _playerInstrumentSeatState = playerInstrumentSeatState;
            _musicInstrument = musicInstrument;
            _stateMachine = stateMachine;
            _rhythmInstrumentState = rhythmInstrumentState;
            _cameraInputRotator = cameraInputRotator;
        }
        
        private void Start()
        {
            _defaultHeight = transform.localPosition.y;
            _hideHeight = _defaultHeight - 1;
            _rhythmModeLever.SetValue(0, 0, 1);
            _rhythmModeLever.OnValueChanged += OnRhythmModeLever;
            transform.localPosition = new Vector3(transform.localPosition.x, _hideHeight, transform.localPosition.z);
            gameObject.SetActive(false);
            _rhythmPianoControlPanelView.OnPlayPressed += OnPlayPressed;
        }

        private void OnDestroy()
        {
            _rhythmModeLever.OnValueChanged -= OnRhythmModeLever;
        }

        public void Show()
        {
            _rhythmModeLever.SetValue(0, 0, 1);
            
            gameObject.SetActive(true);
            _tween?.Kill();
            _tween = transform.DOLocalMoveY(_defaultHeight, 0.5f);
        }
        
        public void Hide()
        {
            _tween?.Kill();
            _tween = transform.DOLocalMoveY(_hideHeight, 0.5f).OnComplete(() => gameObject.SetActive(false));
            _rhythmPianoControlPanelView.Hide();
        }

        private void OnPlayPressed()
        {
            ((IContinuousInteractable)_rhythmModeLever).OnEndInteraction();
            _playerStateMachine.ChangeState(_playerInstrumentSeatState);
            _stateMachine?.ChangeState(_rhythmInstrumentState);
            _cameraInputRotator.SetLimit(_musicInstrument.SeatPoint, _musicInstrument.SheetViewAngleY, _musicInstrument.SheetViewAngleX);
        }
        
        private void OnRhythmModeLever(int value)
        {
            if(value == 0)
            {
                ((IContinuousInteractable)_rhythmModeLever).OnEndInteraction();
                _playerStateMachine.ChangeState(_playerInstrumentSeatState);
                _rhythmPianoControlPanelView.Hide();
            }
            else
            {
                _playerMenuState.SetStandPoint(_musicInstrument.SeatPoint);
                ((IContinuousInteractable)_rhythmModeLever).OnEndInteraction();
                _playerStateMachine.ChangeState(_playerMenuState);
                _rhythmPianoControlPanelView.Show();
            }
        }
    }
}