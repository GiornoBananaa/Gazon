using System;
using System.Collections.Generic;
using DG.Tweening;

namespace Game.Runtime.Weather.WeatherTween
{
    public class WeatherTweener : IDisposable
    {
        private readonly Dictionary<WeatherEntityType, Dictionary<WeatherPropertyType, Tween>> _currentTweens = new();
        private readonly Dictionary<WeatherEntityType, WeatherEntityTweener> _entityTweeners = new()
        {
            {WeatherEntityType.Wind, new WindTweener()}
        };

        public Tween Apply(WeatherTween weatherTween)
        {
            if (!_currentTweens.ContainsKey(weatherTween.WeatherEntity))
                _currentTweens[weatherTween.WeatherEntity] = new Dictionary<WeatherPropertyType, Tween>();
            if(_currentTweens[weatherTween.WeatherEntity].ContainsKey(weatherTween.WeatherProperty))
                _currentTweens[weatherTween.WeatherEntity][weatherTween.WeatherProperty].Kill();
            Tween tween = _entityTweeners[weatherTween.WeatherEntity].Apply(weatherTween);
            _currentTweens[weatherTween.WeatherEntity][weatherTween.WeatherProperty] = tween;
            return tween;
        }

        public void Stop(WeatherEntityType entityType)
        {
            if(!_currentTweens.TryGetValue(entityType, out var propertyTweens)) return;
            foreach (var propertyType in propertyTweens)
            {
                propertyType.Value.Kill();
            }
        }
        
        public void Stop(WeatherEntityType entityType, WeatherPropertyType propertyType)
        {
            if(!_currentTweens.ContainsKey(entityType) || !_currentTweens[entityType].ContainsKey(propertyType)) return;
            _currentTweens[entityType][propertyType].Kill();
        }
        
        public void Dispose()
        {
            foreach (var property in _currentTweens)
            {
                foreach (var tween in property.Value)
                {
                    tween.Value.Kill();
                }
            }
            _entityTweeners.Clear();
            _currentTweens.Clear();
        }
    }
}