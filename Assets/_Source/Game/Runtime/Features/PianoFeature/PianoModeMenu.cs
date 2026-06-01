using System.Linq;
using DG.Tweening;
using Game.Runtime.MusicInstrumentSystem;
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

        private float _defaultHeight;
        private float _hideHeight;
        private Tween _tween;
        
        private void Start()
        {
            _defaultHeight = transform.localPosition.y;
            _hideHeight = _defaultHeight - 1;
            _rhythmModeLever.SetValue(0, 0, 1);
            _rhythmModeLever.OnValueChanged += OnRhythmModeLever;
            transform.localPosition = new Vector3(transform.localPosition.x, _hideHeight, transform.localPosition.z);
            gameObject.SetActive(false);
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
        
        private void OnRhythmModeLever(int value)
        {
            if(value == 0)
                _rhythmPianoControlPanelView.Hide();
            else
                _rhythmPianoControlPanelView.Show();
        }
    }
}