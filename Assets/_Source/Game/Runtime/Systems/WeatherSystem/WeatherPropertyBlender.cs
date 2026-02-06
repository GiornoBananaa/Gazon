using System.Collections.Generic;
using DOTweenConfigs;
using Game.Runtime.WeatherSystem.WeatherTween;
using UnityEngine;

namespace Game.Runtime.WeatherSystem
{
    public class WeatherPropertyBlender
    {
        private class BlendProperty
        {
            public WeatherProperty WeatherProperty;
            public float Weight;

            public BlendProperty(WeatherProperty weatherProperty, float weight)
            {
                WeatherProperty = weatherProperty;
                Weight = weight;
            }
        }
        
        private readonly WeatherTweener _weatherTweener;
        
        private readonly Dictionary<WeatherPropertyType, Dictionary<IWeatherPropertySetter, BlendProperty>> _blendProperties = new();
        private readonly Dictionary<WeatherPropertyType, WeatherProperty> _properties = new();
        private readonly TweenConfig _defaultTweenSettings;
        
        public WeatherPropertyBlender(WeatherTweener weatherTweener)
        {
            _weatherTweener = weatherTweener;
            
            _defaultTweenSettings = new TweenConfig()
            {
                Duration = 1f
            };
        }

        public void ApplyProperty(IWeatherPropertySetter setter, WeatherProperty weatherProperty, float weight)
        {
            WeatherPropertyType weatherPropertyType = weatherProperty.Type;
            
            if (!_blendProperties.ContainsKey(weatherPropertyType))
                _blendProperties[weatherPropertyType] = new Dictionary<IWeatherPropertySetter, BlendProperty>();
            
            if(_blendProperties[weatherPropertyType].ContainsKey(setter))
            {
                WeatherProperty lastProperty = _blendProperties[weatherPropertyType][setter].WeatherProperty;

                if (Mathf.Approximately(lastProperty.FloatValue, weatherProperty.FloatValue)
                    && lastProperty.VectorValue == weatherProperty.VectorValue
                    && lastProperty.ColorValue == weatherProperty.ColorValue)
                    return;
            }
                    
            if(!_properties.ContainsKey(weatherPropertyType))
                _properties[weatherPropertyType] = new WeatherProperty(){Type = weatherPropertyType};

            WeatherProperty result = _properties[weatherPropertyType];
            
            _blendProperties[weatherPropertyType][setter] = new BlendProperty(weatherProperty, weight);

            bool first = true;
            foreach (var blendProperty in _blendProperties[weatherPropertyType].Values)
            {
                if(first)
                {
                    result.FloatValue = blendProperty.WeatherProperty.FloatValue;
                    result.VectorValue = blendProperty.WeatherProperty.VectorValue;
                    result.ColorValue = blendProperty.WeatherProperty.ColorValue;
                }
                else
                {
                    result.FloatValue = Mathf.Lerp(result.FloatValue, blendProperty.WeatherProperty.FloatValue, blendProperty.Weight);
                    result.VectorValue = Vector3.Lerp(result.VectorValue, blendProperty.WeatherProperty.VectorValue, blendProperty.Weight);
                    result.ColorValue = Color.Lerp(result.ColorValue, blendProperty.WeatherProperty.ColorValue, blendProperty.Weight);
                }

                first = false;
            }
            
            _weatherTweener.Apply(new WeatherSystem.WeatherTween.WeatherTween()
            {
                Type = result.Type,
                FloatSettings = new ToTweenConfig<float>(result.FloatValue, _defaultTweenSettings),
                ColorSettings = new ToTweenConfig<Color>(result.ColorValue, _defaultTweenSettings),
                VectorSettings = new ToTweenConfig<Vector2>(result.VectorValue, _defaultTweenSettings),
            });
        }
        
        public void RemoveProperty(IWeatherPropertySetter setter, WeatherPropertyType type)
        {
            
        }
        
        public void RemoveProperties(IWeatherPropertySetter setter)
        {
            
        }
    }
}