using System;
using Game.Runtime.Planet;
using DG.Tweening;
using DOTweenConfigs;
using UnityEngine;

namespace Game.Runtime.Weather.WeatherTween
{
    public class WeatherBiomeSetter
    {
        private readonly WeatherTweener _weatherTweener;
        private readonly TweenConfig _defaultTweenSettings;
        
        public WeatherBiomeSetter(WeatherTweener weatherTweener)
        {
            _weatherTweener = weatherTweener;
            _defaultTweenSettings = new TweenConfig()
            {
                Duration = 1f
            };
        }

        public void SetBiomeWeather(Biome biome)
        {
            foreach (var weatherProperty in biome.WeatherState.Properties)
            {
                _weatherTweener.Apply(new WeatherTween()
                {
                    WeatherEntity = weatherProperty.Entity,
                    WeatherProperty = weatherProperty.Property,
                    FloatSettings = new ToTweenConfig<float>(weatherProperty.FloatValue, _defaultTweenSettings),
                    ColorSettings = new ToTweenConfig<UnityEngine.Color>(weatherProperty.ColorValue, _defaultTweenSettings),
                    VectorSettings = new ToTweenConfig<UnityEngine.Vector2>(weatherProperty.VectorValue, _defaultTweenSettings),
                });
            }
        }
    }
}