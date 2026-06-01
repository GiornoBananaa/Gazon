using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Runtime.PlayerInteractionSystem;
using UnityEngine;
using Game.Runtime.Utils;

namespace Game.Runtime.PianoFeature
{
    public class CircularList : MonoBehaviour, IContinuousInteractable, ILookChangedListener
    {
        [SerializeField] private bool _horizontal = true;
        [SerializeField] private bool _inverted;
        [SerializeField] private float _step = 0.05f;
        [SerializeField] private Vector3 _size = new(0.2f, 0.08f, 0.2f);
        [SerializeField] private float _visibleOffset = 0.3f;
        [SerializeField] private float _sensitivity = 0.01f;
        [SerializeField] private float _animationMetersBySecond = 1f;
        [SerializeField] private ListOption _optionPrefab;
        
        private readonly List<ListOption> _options = new();
        private float _value;
        private Tween _tween;
        private bool _initialized;
        
        public int Value { get; private set; }
        
        public event Action<int> OnValueChanged;
        
        private void Awake()
        {
            if(!_initialized)
                Initialize();
        }

        private void Initialize()
        {
            _initialized = true;
        }
        
        public void SetOptions(string[] names)
        {
            if(!_initialized)
                Initialize();
            _options.Clear();
            
            for (int i = 0; i < names.Length &&  i < _options.Count; i++)
            {
                _options[i].gameObject.SetActive(true);
                _options[i].SetText(names[i]);
                _options[i].SetSize(_size);
            }
            for (int i = _options.Count; i < names.Length; i++)
            {
                var option = Instantiate(_optionPrefab, transform);
                option.SetText(names[i]);
                _options.Add(option);
                _options[i].SetSize(_size);
            }
            for (int i = names.Length; i < _options.Count; i++)
            {
                _options[i].gameObject.SetActive(false);
            }
            
            _value = 0;
            Value = 0;
            UpdatePosition();
        }

        void IContinuousInteractable.OnStartInteraction()
        {
            
        }
        
        void IContinuousInteractable.OnEndInteraction()
        {
            _value = Value;
            ValueChangeAnimation(Value);
        }
        
        void ILookChangedListener.OnLookChanged(Vector2 look)
        {
            if(_tween != null)
                _tween.Kill();
            _value += (_horizontal ? look.x : look.y) * _sensitivity;
            UpdatePosition();

            int newValue = Mathf.RoundToInt(MathUtils.Repeat(_value, - 0.49f, _options.Count - 0.51f));
            if(newValue != Value)
            {
                Value = newValue;
                OnValueChanged?.Invoke(Value);
            }
        }

        private void UpdatePosition()
        {
            for (int i = 0; i < _options.Count; i++)
            {
                float value = MathUtils.Repeat(_value - i, -_options.Count / 2f, _options.Count / 2f);
                _options[i].transform.localPosition = GetOptionPosition(value);
                bool visible = Mathf.Abs(value * _step) <= _visibleOffset;
                _options[i].gameObject.SetActive(visible);
            }
        }

        private Vector3 GetOptionPosition(float value)
        {
            float offset = value * _step;
            Vector3 position;
            if(_horizontal)
                position = new Vector3(offset, 0, 0);
            else
                position = new Vector3(0, offset, 0);
            return position;
        }
        
        private void ValueChangeAnimation(int value)
        {
            _tween?.Kill();
            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < _options.Count; i++)
            {
                var optionTransform = _options[i].transform;
                float positionIndex = Mathf.RoundToInt(MathUtils.Repeat(value - i, -_options.Count / 2f, _options.Count / 2f));
                Vector2 optionPosition = GetOptionPosition(positionIndex);
                float path = Vector3.Distance(optionPosition, optionTransform.localPosition);
                
                if (path > _step * 1.5f)
                    optionTransform.localPosition = optionPosition;
                else
                    sequence.Join(_options[i].transform.DOLocalMove(optionPosition, path / _animationMetersBySecond));
                
                bool visible = Mathf.Abs(positionIndex * _step) <= _visibleOffset;
                _options[i].gameObject.SetActive(visible);
            }
            _tween = sequence;
        }
    }
}