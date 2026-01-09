using System;
using System.Collections.Generic;
using Game.Runtime.Planet;
using DOTweenConfigs;
using Game.Runtime.CameraFeature;
using Game.Runtime.Configs;
using Game.Runtime.Planet.Movement;
using Game.Runtime.Utils;
using UnityEngine;

namespace Game.Runtime.Weather.WeatherTween
{
    public class WeatherBiomeSetter
    {
        private class BlendProperty
        {
            public WeatherProperty WeatherProperty;
            public bool Used;
        }
        
        private readonly WeatherTweener _weatherTweener;
        private readonly PlanetMap _planetMap;
        private readonly ICurrentCamera _currentCamera;
        
        private readonly TweenConfig _defaultTweenSettings;
        private readonly Dictionary<Biome, float> _biomesBlend = new();
        private readonly Dictionary<WeatherEntityPropertyType, BlendProperty> _properties = new();
        private readonly Dictionary<WeatherEntityPropertyType, BlendProperty> _lastProperties = new();
        private readonly float _blendDistance;
        
        public WeatherBiomeSetter(WeatherTweener weatherTweener, PlanetMap planetMap, ICurrentCamera camera)
        {
            _weatherTweener = weatherTweener;
            _planetMap = planetMap;
            _currentCamera = camera;
            
            _defaultTweenSettings = new TweenConfig()
            {
                Duration = 1f
            };

            _blendDistance = RootConfig.Instance.PlanetConfig.ChunkWeatherBlendDistance;
        }

        public void UpdateBiomeWeather()
        {
            _biomesBlend.Clear();
            
            float size = _planetMap.ChunkSize.CurrentValue / 2;
            float blendSum = 0;
            
            foreach (var neighbor in ArrayUtils.GetNeighborIndexesWithoutBorders(_planetMap.Chunks.CurrentValue.GetLength(0), _planetMap.Chunks.CurrentValue.GetLength(1),
                         _planetMap.CurrentChunkIndex.CurrentValue.x, _planetMap.CurrentChunkIndex.CurrentValue.y, true))
            {
                float distanceX = Mathf.Abs(_planetMap.GetChunkCenterPosition(neighbor).x - _currentCamera.GetCurrentCamera().transform.position.x);
                float distanceY = Mathf.Abs(_planetMap.GetChunkCenterPosition(neighbor).z - _currentCamera.GetCurrentCamera().transform.position.z);

                float blend = Mathf.InverseLerp(size + _blendDistance, size - _blendDistance, MathF.Max(distanceX, distanceY));

                blend = Mathf.Clamp01(blend);
                
                Biome biome = _planetMap.GetBiomeByChunk(neighbor);
                
                if(blend <= 0 || _biomesBlend.ContainsKey(biome) && _biomesBlend[biome] > blend) continue;
                
                if(!_biomesBlend.ContainsKey(biome))
                    blendSum += blend;
                
                _biomesBlend[biome] = blend;
            }
            
            foreach (var property in _properties.Values)
            {
                property.Used = false;
            }
            
            foreach (var biomeBlend in _biomesBlend)
            {
                float blend = biomeBlend.Value / blendSum;
                
                foreach (var property in biomeBlend.Key.WeatherState.Properties)
                {
                    WeatherEntityPropertyType propertyTypeEntity = new WeatherEntityPropertyType(property.EntityType, property.PropertyType);
                    if (!_properties.ContainsKey(propertyTypeEntity))
                    {
                        _properties[propertyTypeEntity] = new BlendProperty()
                        {
                            WeatherProperty = new WeatherProperty()
                            {
                                EntityType = property.EntityType,
                                PropertyType = property.PropertyType
                            }
                        };
                        
                        _lastProperties[propertyTypeEntity] = new BlendProperty()
                        {
                            WeatherProperty = new WeatherProperty()
                            {
                                EntityType = property.EntityType,
                                PropertyType = property.PropertyType
                            }
                        };
                    }
                    
                    if(!_properties[propertyTypeEntity].Used)
                    {
                        _properties[propertyTypeEntity].WeatherProperty.FloatValue = property.FloatValue;
                        _properties[propertyTypeEntity].WeatherProperty.VectorValue = property.VectorValue;
                        _properties[propertyTypeEntity].WeatherProperty.ColorValue = property.ColorValue;
                        _properties[propertyTypeEntity].Used = true;
                    }
                    else
                    {
                        _properties[propertyTypeEntity].WeatherProperty.FloatValue = Mathf.Lerp(_properties[propertyTypeEntity].WeatherProperty.FloatValue, property.FloatValue, blend);
                        _properties[propertyTypeEntity].WeatherProperty.VectorValue =  Vector3.Lerp(_properties[propertyTypeEntity].WeatherProperty.VectorValue, property.VectorValue, blend);
                        _properties[propertyTypeEntity].WeatherProperty.ColorValue = Color.Lerp(_properties[propertyTypeEntity].WeatherProperty.ColorValue, property.ColorValue, blend);
                    }
                }
            }
            
            foreach (var property in _properties.Values)
            {
                if(!property.Used) continue;
                
                var lastProperty = _lastProperties[new() { Entity = property.WeatherProperty.EntityType, Property = property.WeatherProperty.PropertyType }].WeatherProperty;
                if(Mathf.Approximately(lastProperty.FloatValue, property.WeatherProperty.FloatValue) 
                   && lastProperty.VectorValue == property.WeatherProperty.VectorValue 
                   && lastProperty.ColorValue == property.WeatherProperty.ColorValue) continue;
                
                ApplyProperty(property.WeatherProperty);

                lastProperty.FloatValue = property.WeatherProperty.FloatValue;
                lastProperty.VectorValue = property.WeatherProperty.VectorValue;
                lastProperty.ColorValue = property.WeatherProperty.ColorValue;
            }
        }

        private void ApplyProperty(WeatherProperty weatherProperty)
        {
            _weatherTweener.Apply(new WeatherTween()
            {
                WeatherEntity = weatherProperty.EntityType,
                WeatherProperty = weatherProperty.PropertyType,
                FloatSettings = new ToTweenConfig<float>(weatherProperty.FloatValue, _defaultTweenSettings),
                ColorSettings = new ToTweenConfig<Color>(weatherProperty.ColorValue, _defaultTweenSettings),
                VectorSettings = new ToTweenConfig<Vector2>(weatherProperty.VectorValue, _defaultTweenSettings),
            });
        }
    }
}