using System;
using DG.Tweening;
using DOTweenConfigs;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Runtime.WeatherSystem.WeatherTween
{
    [Serializable]
    public class WeatherTween
    {
        public WeatherPropertyType Type;
        
        public ToTweenConfig<float> FloatSettings;
        public ToTweenConfig<Vector2> VectorSettings;
        public ToTweenConfig<Color> ColorSettings;
        
        
        public Tween ApplyTo<T>(ITweenTarget<T> target)
        {
            if (target is ITweenTarget<float> targetFloat)
            {
                return DOTween.To(() => targetFloat.GetWeatherTweenValue(), v => targetFloat.SetWeatherTweenValue(v), FloatSettings.To, FloatSettings.Duration);
            }
            if (target is ITweenTarget<Vector2> targetVector) 
                return DOTween.To(() => targetVector.GetWeatherTweenValue(), v => targetVector.SetWeatherTweenValue(v), VectorSettings.To, FloatSettings.Duration);
            if (target is ITweenTarget<Color> targetColor) 
                return DOTween.To(() => targetColor.GetWeatherTweenValue(), v => targetColor.SetWeatherTweenValue(v), ColorSettings.To, FloatSettings.Duration);
            
            throw new ArgumentException("Invalid weather property type");
        }
    }
}