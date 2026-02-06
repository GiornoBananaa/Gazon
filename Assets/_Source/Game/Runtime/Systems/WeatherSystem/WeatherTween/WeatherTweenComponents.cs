using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Runtime.WeatherSystem.WeatherTween
{
    [Serializable]
    public class WeatherState
    {
        public WeatherProperty[] Properties;
    }
    
    [Serializable]
    public struct WeatherProperty
    { 
        public WeatherPropertyType Type;
        
        public float FloatValue;
        public Vector2 VectorValue;
        public Color ColorValue;
    }
    
    [Serializable]
    public struct WeatherPropertyType : IEquatable<WeatherPropertyType>
    {
        public WeatherEntityType Entity;
        public WeatherParameterType Parameter;

        public WeatherPropertyType(WeatherEntityType entity , WeatherParameterType parameter)
        {
            Entity = entity;
            Parameter = parameter;
        }
        
        public bool Equals(WeatherPropertyType other)
        {
            return Entity == other.Entity && Parameter == other.Parameter;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Entity, (int)Parameter);
        }
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
            switch (tween.Type.Parameter)
            {
                case WeatherParameterType.Force: return TweenForce(tween);
                case WeatherParameterType.Direction: return TweenDirection(tween);
                case WeatherParameterType.Position: return TweenPosition(tween);
                case WeatherParameterType.Color: return TweenColor(tween);
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

    public enum WeatherParameterType
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