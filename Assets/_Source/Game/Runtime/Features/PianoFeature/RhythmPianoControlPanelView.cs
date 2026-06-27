using System;
using System.Linq;
using DG.Tweening;
using Game.Runtime.MusicInstrumentSystem;
using Game.Runtime.StateMachineSystem;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Runtime.PianoFeature
{
    public class RhythmPianoControlPanelView : MonoBehaviour
    {
        [SerializeField] private RotationSwitch _keysCountSwitch;
        [SerializeField] private RotationSwitch _maxSpeedSwitch;
        [SerializeField] private RotationSwitch _maxNotesPerSecondSwitch;
        [SerializeField] private RotationSwitch _launchGameLever;
        [SerializeField] private CircularList _trackList;
        
        private RhythmGameSettings _rhythmGameSettings;
        private MusicLibrary _library;
        private Tween _tween;
        private float _defaultHeight;
        private float _hideHeight;

        public event Action OnPlayPressed;
        
        [Inject]
        private void Construct(RhythmGameSettings settings, MusicLibrary library)
        {
            _library = library;
            _rhythmGameSettings = settings;
        }

        private void Awake()
        {
            _defaultHeight = transform.localPosition.y;
            _hideHeight = _defaultHeight - 1;
            _keysCountSwitch.SetValue(_rhythmGameSettings.KeysCount, GlobalConstants.MIN_KEYS_COUNT, GlobalConstants.MAX_KEYS_COUNT);
            _maxSpeedSwitch.SetValue((int)(_rhythmGameSettings.MusicSpeed*10), (int)(GlobalConstants.MIN_MUSIC_SPEED*10), (int)(GlobalConstants.MAX_MUSIC_SPEED*10));
            _maxNotesPerSecondSwitch.SetValue((int)(_rhythmGameSettings.MaxNotesPerSecond), GlobalConstants.MIN_KEYS_PER_SECOND, GlobalConstants.MAX_KEYS_PER_SECOND);
            _launchGameLever.SetValue(0, 0, 1);
            
            _keysCountSwitch.OnValueChanged += OnKeysCountChanged;
            _maxSpeedSwitch.OnValueChanged += OnSpeedChanged;
            _maxNotesPerSecondSwitch.OnValueChanged += MaxNotesPerSecondChanged;
            _launchGameLever.OnValueChanged += OnLaunchLever;
            _trackList.OnValueChanged += OnTrackChanged;
            
            transform.localPosition = new Vector3(transform.localPosition.x, _hideHeight, transform.localPosition.z);
        }

        private void Start()
        {
            _trackList.SetOptions(_library.MusicTracks.Select(t => t.Name).ToArray());
        }

        public void Show()
        {
            _launchGameLever.SetValue(0, 0, 1);
            
            _tween?.Kill();
            _tween = transform.DOLocalMoveY(_defaultHeight, 0.5f);
        }
        
        public void Hide()
        {
            _tween?.Kill();
            _tween = transform.DOLocalMoveY(_hideHeight, 0.5f);
        }
        
        private void OnKeysCountChanged(int value)
        {
            _rhythmGameSettings.KeysCount = value;
        }
        
        private void OnSpeedChanged(int value)
        {
            _rhythmGameSettings.MusicSpeed = value/10f;
        }
        
        private void MaxNotesPerSecondChanged(int value)
        {
            _rhythmGameSettings.MaxNotesPerSecond = value;
        }
        
        private void OnLaunchLever(int value)
        {
            if(value == 0) return;
            OnPlayPressed?.Invoke();
        }
        
        private void OnTrackChanged(int value)
        {
            _rhythmGameSettings.MusicTrackId = value;
        }
        
        private void OnDestroy()
        {
            _keysCountSwitch.OnValueChanged -= OnKeysCountChanged;
            _maxSpeedSwitch.OnValueChanged -= OnSpeedChanged;
            _maxNotesPerSecondSwitch.OnValueChanged -= MaxNotesPerSecondChanged;
            _launchGameLever.OnValueChanged -= OnLaunchLever;
            _trackList.OnValueChanged -= OnTrackChanged;
        }
    }
}