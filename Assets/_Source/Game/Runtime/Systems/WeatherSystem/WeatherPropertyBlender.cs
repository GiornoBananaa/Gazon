using System.Collections.Generic;
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
        private readonly Dictionary<WeatherPropertyType, float> _weights = new();
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
            
            _blendProperties[weatherPropertyType][setter] = new BlendProperty(weatherProperty, weight);
            _weights.TryAdd(weatherPropertyType, 0);
            _weights[weatherPropertyType] += _blendProperties[weatherPropertyType][setter].Weight;
            
            UpdateProperty(weatherPropertyType);
        }
        
        public void RemoveProperty(IWeatherPropertySetter setter, WeatherPropertyType type)
        {
            _weights[type] -= _blendProperties[type][setter].Weight;
            _blendProperties[type].Remove(setter);
            UpdateProperty(type);
        }
        
        public void RemoveProperties(IWeatherPropertySetter setter)
        {
            foreach (var pair in _blendProperties)
            {
                _weights[pair.Key] -= _blendProperties[pair.Key][setter].Weight;
                _blendProperties[pair.Key].Remove(setter);
                UpdateProperty(pair.Key);
            }
        }

        private void UpdateProperty(WeatherPropertyType weatherPropertyType)
        {
            WeatherProperty result = _properties[weatherPropertyType];
            
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
                    result.FloatValue = Mathf.Lerp(result.FloatValue, blendProperty.WeatherProperty.FloatValue, blendProperty.Weight / _weights[weatherPropertyType]);
                    result.VectorValue = Vector3.Lerp(result.VectorValue, blendProperty.WeatherProperty.VectorValue, blendProperty.Weight / _weights[weatherPropertyType]);
                    result.ColorValue = Color.Lerp(result.ColorValue, blendProperty.WeatherProperty.ColorValue, blendProperty.Weight / _weights[weatherPropertyType]);
                }

                first = false;
            }
            
            _weatherTweener.Apply(new WeatherSystem.WeatherTween.WeatherTween()
            {
                Type = result.Type,
                TweenConfig = _defaultTweenSettings,
                FloatValue = result.FloatValue,
                ColorValue = result.ColorValue,
                VectorValue = result.VectorValue,
            });
        }
    }
}