using System;
using DG.Tweening;
using UnityEngine;

namespace Game.Runtime.WeatherSystem.WeatherTween
{
    [Serializable]
    public class WeatherTween
    {
        public WeatherPropertyType Type;
        
        public float FloatValue;
        public Vector3 VectorValue;
        public Color ColorValue;
        
        public TweenConfig TweenConfig;

        private Tween _tween;
        
        public WeatherTween() { }
        
        public WeatherTween(WeatherTween tween)
        {
            Type = tween.Type;
            FloatValue = tween.FloatValue;
            VectorValue = tween.VectorValue;
            ColorValue = tween.ColorValue;
            TweenConfig = tween.TweenConfig;
        }
        
        public Tween ApplyTo<T>(ITweenTarget<T> target)
        {
            Tween tween = null;
            float duration = TweenConfig.Duration;
            if (typeof(T) == typeof(float) && target is ITweenTarget<float> targetFloat)
            {
                if (TweenConfig.DurationAsSpeedEnabled)
                    duration = Mathf.Abs(targetFloat.GetTweenValue() - FloatValue) / TweenConfig.Duration;
                
                tween = DOTween.To(() => targetFloat.GetTweenValue(), v => targetFloat.SetTweenValue(v), FloatValue, duration);
            }
            else if (typeof(T) == typeof(Vector2) && target is ITweenTarget<Vector2> targetVector)
            {
                if (TweenConfig.DurationAsSpeedEnabled)
                    duration = (targetVector.GetTweenValue() - (Vector2)VectorValue).magnitude / TweenConfig.Duration;
                tween = DOTween.To(() => targetVector.GetTweenValue(), v => targetVector.SetTweenValue(v), (Vector2)VectorValue, duration);
            }
            else if (typeof(T) == typeof(Color) && target is ITweenTarget<Color> targetColor)
            {
                if(TweenConfig.DurationAsSpeedEnabled)
                {
                    var start = targetColor.GetTweenValue();
                    float length = Mathf.Max(Mathf.Max(Mathf.Abs(ColorValue.r - start.r), Mathf.Abs(ColorValue.g - start.g)), 
                        Mathf.Max(Mathf.Abs(ColorValue.b - start.b), Mathf.Abs(ColorValue.a - start.a)));
                    duration = length / TweenConfig.Duration;
                }
                
                tween = DOTween.To(() => targetColor.GetTweenValue(), v => targetColor.SetTweenValue(v), ColorValue, duration);
            }
            else if(typeof(T) == typeof(Quaternion) && target is ITweenTarget<Quaternion> targetQuaternion)
            {
                if(TweenConfig.DurationAsSpeedEnabled) 
                    duration = Quaternion.Angle(targetQuaternion.GetTweenValue(), Quaternion.Euler(VectorValue)) / TweenConfig.Duration;
                
                var t = DOTween.To(() => targetQuaternion.GetTweenValue(), v => targetQuaternion.SetTweenValue(v), VectorValue, duration);
                t.plugOptions.rotateMode = RotateMode.Fast;
                tween = t;
            }
            
            if(tween == null)
             throw new ArgumentException($"{target.GetType()} has invalid weather property type");
            
            if(TweenConfig.CustomEaseEnabled)
                tween.SetEase(TweenConfig.CustomEase);
            else
                tween.SetEase(TweenConfig.Ease);
            
            return tween;
        }
    }
}