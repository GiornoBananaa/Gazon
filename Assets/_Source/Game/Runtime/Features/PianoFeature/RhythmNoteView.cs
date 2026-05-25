using DG.Tweening;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class RhythmNoteView : MonoBehaviour
    {
        [SerializeField] private Transform _noteBody;
        [SerializeField] private Transform _stand;
        [SerializeField] private MeshRenderer _meshRenderer;
        
        private bool _highlight;
        private Color _baseColor;
        private Color _highlightColor;
        
        public void SetNoteLength(float length)
        {
            _noteBody.localScale = new Vector3(_noteBody.localScale.x, _noteBody.localScale.y, length);
            _noteBody.localPosition = new Vector3(_noteBody.localPosition.x, _noteBody.localPosition.y, length / 2);
            _stand.localPosition = new Vector3(_stand.localPosition.x, _stand.localPosition.y, length / 2);
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