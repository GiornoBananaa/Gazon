using DG.Tweening;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class RhythmTrackView : MonoBehaviour
    {
        [SerializeField] private Transform _body;
        [SerializeField] private MeshRenderer _meshRenderer;
        
        private bool _highlight;
        private Color _baseColor;
        private Color _highlightColor;
        private Tween _tweenColor;
        
        public void SetTrackSize(float length, float width)
        {
            _body.localScale = new Vector3(width, _body.localScale.y, length);
            _body.localPosition = new Vector3(_body.localPosition.x, _body.localPosition.y, length / 2);
            _body.localPosition = new Vector3(_body.localPosition.x, _body.localPosition.y, length / 2);
        }
        
        public void SetBaseColor(Color color)
        {
            _baseColor = color;
            if(!_highlight)
                _meshRenderer.material.color = _baseColor;
        }
        
        public void SetHighlightColor(Color color)
        {
            _highlightColor = color;
            if (_highlight)
                SwitchColor();
        }
        
        public void Highlight(bool isOn)
        {
            if(_highlight == isOn) return;
            _highlight = isOn;
            SwitchColor();
        }
        
        public void HighlightBlink()
        {
            _tweenColor?.Kill();
            
            _tweenColor = DOTween.Sequence()
                .Append(_meshRenderer.material.DOColor(_highlightColor, 0.2f))
                .Append(_meshRenderer.material.DOColor(_baseColor, 0.5f));
        }

        private void SwitchColor()
        {
            _meshRenderer.material.DOColor(_highlight ? _highlightColor : _baseColor, 0.1f);
        }
        
        public void ResetTrack()
        {
            _highlight = false;
            transform.DOKill();
            _meshRenderer.material.color = _baseColor;
        }
    }
}