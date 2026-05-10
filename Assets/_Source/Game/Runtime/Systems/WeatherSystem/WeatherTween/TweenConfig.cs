using System;
using DG.Tweening;
using UnityEngine;

namespace Game.Runtime.WeatherSystem.WeatherTween
{
    [Serializable]
    public struct TweenConfig
    {
        public bool DurationAsSpeedEnabled;
        public float Duration;
        public bool CustomEaseEnabled;
        public Ease Ease;
        public AnimationCurve CustomEase;
        
        public TweenConfig(TweenConfig configuration)
        {
            DurationAsSpeedEnabled = configuration.DurationAsSpeedEnabled;
            Duration = configuration.Duration;
            CustomEaseEnabled = configuration.CustomEaseEnabled;
            Ease = configuration.Ease;
            CustomEase = configuration.CustomEase;
        }
    }
}
