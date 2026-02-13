using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Runtime.WeatherSystem.WeatherTween
{
    public interface ITweenTarget<T>
    {
        public T GetWeatherTweenValue();
        public void SetWeatherTweenValue(T value);
    }
    
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