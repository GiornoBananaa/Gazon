using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game.Runtime.Weather.WeatherTween
{
    [Serializable]
    public class WeatherState
    {
        public WeatherProperty[] Properties;
    }
    
    [Serializable]
    public class WeatherProperty
    {
        public WeatherEntityType Entity;
        public WeatherPropertyType Property;
        
        public float FloatValue;
        public Vector2 VectorValue;
        public Color ColorValue;
    }

    public interface ITweenTarget<T>
    {
        public T GetWeatherTweenValue();
        public void SetWeatherTweenValue(T value);
    }

    public abstract class WeatherEntityTweener
    {
        public Tween Apply(WeatherTween tween)
        {
            switch (tween.WeatherProperty)
            {
                case WeatherPropertyType.Force: return TweenForce(tween);
                case WeatherPropertyType.Direction: return TweenDirection(tween);
                case WeatherPropertyType.Position: return TweenPosition(tween);
                case WeatherPropertyType.Color: return TweenColor(tween);
            }
            throw new ArgumentException("Invalid weather property type");
        }

        protected virtual Tween TweenForce(WeatherTween tween) => throw new NotImplementedException();
        protected virtual Tween TweenDirection(WeatherTween tween) => throw new NotImplementedException();
        protected virtual Tween TweenPosition(WeatherTween tween) => throw new NotImplementedException();
        protected virtual Tween TweenColor(WeatherTween tween) => throw new NotImplementedException();
        
        protected Tween GetSequence<T>(WeatherTween tween, IEnumerable<ITweenTarget<T>> targets)
        {
            var sequence = DOTween.Sequence();
            foreach (var target in targets)
            {
                sequence.Join(tween.ApplyTo(target));
            }
            
            return sequence;
        }
    }

    public enum WeatherPropertyType
    {
        None = 0,
        Force = 1,
        Direction = 2,
        Position = 3,
        Color = 4,
    }
    
    public enum WeatherEntityType
    {
        None = 0,
        Wind = 1,
        Grass = 2,
        Sky = 3,
        Sun = 4,
        Moon = 5,
        Light = 6,
        Fog = 7,
        Stars = 8,
    }
}