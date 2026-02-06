using System;
using Game.Runtime.WeatherSystem.WeatherTween;

namespace Game.Runtime.WeatherSystem
{
    public static class WeatherSettingsUtil
    {
        public static WeatherParameterType[] GetProperties(this WeatherEntityType entityType)
        {
            switch (entityType)
            {
                case WeatherEntityType.Wind:
                    return new[]{WeatherParameterType.Force, WeatherParameterType.Direction};
                case WeatherEntityType.Grass:
                    return new[]{WeatherParameterType.Color};
                case WeatherEntityType.Sky:
                    return new[]{WeatherParameterType.Color};
                case WeatherEntityType.Sun:
                    return new[]{WeatherParameterType.Color, WeatherParameterType.Position};
                case WeatherEntityType.Moon:
                    return new[]{WeatherParameterType.Color, WeatherParameterType.Position};
                case WeatherEntityType.Light:
                    return new[]{WeatherParameterType.Color};
                case WeatherEntityType.Fog:
                    return new[]{WeatherParameterType.Force, WeatherParameterType.Color};
                case WeatherEntityType.Stars:
                    return new[]{WeatherParameterType.Force, WeatherParameterType.Color};
                default:
                    return new[]{WeatherParameterType.None};
            }
            return Array.Empty<WeatherParameterType>();
        }
        
        public static Type GetPropertyType(this WeatherParameterType entityType)
        {
            switch (entityType)
            {
                case WeatherParameterType.Force:
                    return typeof(float);
                case WeatherParameterType.Direction:
                    return typeof(UnityEngine.Vector2);
                case WeatherParameterType.Position:
                    return typeof(UnityEngine.Vector2);
                case WeatherParameterType.Color:
                    return typeof(UnityEngine.Color);
            }
            return null;
        }
    }
}