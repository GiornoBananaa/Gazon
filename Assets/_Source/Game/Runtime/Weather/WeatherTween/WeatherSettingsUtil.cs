using System;

namespace Game.Runtime.Weather.WeatherTween
{
    public static class WeatherSettingsUtil
    {
        public static WeatherPropertyType[] GetProperties(this WeatherEntityType entityType)
        {
            switch (entityType)
            {
                case WeatherEntityType.Wind:
                    return new[]{WeatherPropertyType.Force, WeatherPropertyType.Direction};
                case WeatherEntityType.Grass:
                    return new[]{WeatherPropertyType.Color};
                case WeatherEntityType.Sky:
                    return new[]{WeatherPropertyType.Color};
                case WeatherEntityType.Sun:
                    return new[]{WeatherPropertyType.Color, WeatherPropertyType.Position};
                case WeatherEntityType.Moon:
                    return new[]{WeatherPropertyType.Color, WeatherPropertyType.Position};
                case WeatherEntityType.Light:
                    return new[]{WeatherPropertyType.Color};
                case WeatherEntityType.Fog:
                    return new[]{WeatherPropertyType.Force, WeatherPropertyType.Color};
                case WeatherEntityType.Stars:
                    return new[]{WeatherPropertyType.Force, WeatherPropertyType.Color};
            }
            return Array.Empty<WeatherPropertyType>();
        }
        
        public static Type GetPropertyType(this WeatherPropertyType entityType)
        {
            switch (entityType)
            {
                case WeatherPropertyType.Force:
                    return typeof(float);
                case WeatherPropertyType.Direction:
                    return typeof(UnityEngine.Vector2);
                case WeatherPropertyType.Position:
                    return typeof(UnityEngine.Vector2);
                case WeatherPropertyType.Color:
                    return typeof(UnityEngine.Color);
            }
            return null;
        }
    }
}