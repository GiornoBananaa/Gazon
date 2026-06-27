using System;
using DG.Tweening;
using Game.Runtime.PlayerInteractionSystem;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class RotationSwitch : MonoBehaviour, IContinuousInteractable, ILookChangedListener
    {
        [SerializeField] private bool _horizontal = true;
        [SerializeField] private bool _inverted;
        [SerializeField] private float _minRotation = 0f;
        [SerializeField] private float _maxRotation = 360f;
        [SerializeField] private float _sensitivity = 0.01f;
        [SerializeField] private float _animationDegreesBySecond = 720f;
        
        private float _value;
        private Tween _tween;
        private Quaternion _defaultLocalRotation;
        private Vector3 _localForward;
        private bool _initialized;
        
        public int Value { get; private set; }
        public int Min { get; private set; }
        public int Max { get; private set; }
        
        public event Action<int> OnValueChanged;
        
        private void Awake()
        {
            if(!_initialized)
                Initialize();
        }

        private void Initialize()
        {
            _defaultLocalRotation = transform.localRotation;
            _localForward = transform.InverseTransformDirection(transform.forward);
            _initialized = true;
        }
        
        public void SetValue(int value, int min, int max)
        {
            if(!_initialized)
                Initialize();
            Value = value;
            Min = min;
            Max = max;
            _value = Mathf.InverseLerp(_inverted ? Max : Min, _inverted ? Min : Max, value);
            float rotation = Mathf.Lerp(_minRotation, _maxRotation, _value);
            transform.localRotation = _defaultLocalRotation * Quaternion.AngleAxis(rotation, _localForward);
        }

        void IContinuousInteractable.OnStartInteraction()
        {
            
        }
        
        void IContinuousInteractable.OnEndInteraction()
        {
            ValueChangeAnimation(Value);
        }
        
        void ILookChangedListener.OnLookChanged(Vector2 look)
        {
            _value += (_horizontal ? look.x : look.y) * _sensitivity;
            _value = Mathf.Clamp01(_value);
            float rotation = Mathf.Lerp(_minRotation, _maxRotation, _value);
            transform.localRotation = _defaultLocalRotation * Quaternion.AngleAxis(rotation, _localForward);

            int newValue = Mathf.RoundToInt(Mathf.Lerp(_inverted ? Max : Min, _inverted ? Min : Max, _value));
            
            if(newValue != Value)
            {
                Value = newValue;
                OnValueChanged?.Invoke(Value);
            }
        }

        private void ValueChangeAnimation(int value)
        {
            _tween?.Kill();
            float rotation = Mathf.Lerp(_minRotation, _maxRotation, Mathf.InverseLerp(_inverted ? Max : Min, _inverted ? Min : Max, value));
            Quaternion newRotation = _defaultLocalRotation * Quaternion.AngleAxis(rotation, _localForward);
            float angle = Quaternion.Angle(transform.localRotation, newRotation);
            _tween = transform.DOLocalRotate(newRotation.eulerAngles, Mathf.Abs(angle) / _animationDegreesBySecond);
        }
    }
}