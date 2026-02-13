using System;
using System.Collections.Generic;
using Game.Runtime.CameraSystem;
using Game.Runtime.ServiceSystem;
using Game.Runtime.TerrainChunkSystem;
using Game.Runtime.Utils;
using Game.Runtime.WeatherSystem;
using Game.Runtime.WeatherSystem.WeatherTween;
using UnityEngine;

namespace Game.Runtime.WeatherFeature
{
    public class WeatherBiomeSetter : IUpdatable, IWeatherPropertySetter
    {
        private class BlendProperty
        {
            public WeatherProperty WeatherProperty;
            public bool Used;
        }
        
        private readonly WeatherPropertyBlender _weatherPropertyBlender;
        private readonly PlanetMap _planetMap;
        private readonly ICurrentCamera _currentCamera;
        
        private readonly Dictionary<Biome, float> _biomesBlend = new();
        private readonly Dictionary<WeatherPropertyType, BlendProperty> _properties = new();
        private readonly Dictionary<WeatherPropertyType, BlendProperty> _lastProperties = new();
        private readonly Dictionary<Biome, WeatherState> _biomeWeatherStates = new();
        
        private float _blendDistance;
        
        public WeatherBiomeSetter(WeatherPropertyBlender weatherPropertyBlender, PlanetMap planetMap, ICurrentCamera camera)
        {
            _weatherPropertyBlender = weatherPropertyBlender;
            _planetMap = planetMap;
            _currentCamera = camera;
        }

        public void SetData(Dictionary<Biome, WeatherState> biomeWeatherStates, float blendDistance)
        {
            foreach (var state in biomeWeatherStates)
            {
                _biomeWeatherStates.Add(state.Key, state.Value);
            }
            _blendDistance = blendDistance;
        }
        
        public void Update()
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
                
                foreach (var property in _biomeWeatherStates[biomeBlend.Key].Properties)
                {
                    if (!_properties.ContainsKey(property.Type))
                    {
                        _properties[property.Type] = new BlendProperty()
                        {
                            WeatherProperty = property
                        };
                        
                        _lastProperties[property.Type] = new BlendProperty()
                        {
                            WeatherProperty = property
                        };
                    }
                    
                    if(!_properties[property.Type].Used)
                    {
                        _properties[property.Type].WeatherProperty.FloatValue = property.FloatValue;
                        _properties[property.Type].WeatherProperty.VectorValue = property.VectorValue;
                        _properties[property.Type].WeatherProperty.ColorValue = property.ColorValue;
                        _properties[property.Type].Used = true;
                    }
                    else
                    {
                        _properties[property.Type].WeatherProperty.FloatValue = Mathf.Lerp(_properties[property.Type].WeatherProperty.FloatValue, property.FloatValue, blend);
                        _properties[property.Type].WeatherProperty.VectorValue =  Vector3.Lerp(_properties[property.Type].WeatherProperty.VectorValue, property.VectorValue, blend);
                        _properties[property.Type].WeatherProperty.ColorValue = Color.Lerp(_properties[property.Type].WeatherProperty.ColorValue, property.ColorValue, blend);
                    }
                }
            }
            
            foreach (var property in _properties.Values)
            {
                if(!property.Used) continue;
                
                var lastProperty = _lastProperties[property.WeatherProperty.Type].WeatherProperty;
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
            _weatherPropertyBlender.ApplyProperty(this, weatherProperty, 1);
        }
    }
}