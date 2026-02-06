using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Runtime.WeatherSystem.WeatherTween.Tweeners;

namespace Game.Runtime.WeatherSystem.WeatherTween
{
    public class WeatherTweener : IDisposable
    {
        private readonly Dictionary<WeatherEntityType, Dictionary<WeatherParameterType, Tween>> _currentTweens = new();
        private readonly Dictionary<WeatherEntityType, WeatherEntityTweener> _entityTweeners = new()
        {
            { WeatherEntityType.Wind, new WindTweener() }
        };

        public Tween Apply(WeatherTween weatherTween)
        {
            if (!_currentTweens.ContainsKey(weatherTween.Type.Entity))
                _currentTweens[weatherTween.Type.Entity] = new Dictionary<WeatherParameterType, Tween>();
            if(_currentTweens[weatherTween.Type.Entity].ContainsKey(weatherTween.Type.Parameter))
                _currentTweens[weatherTween.Type.Entity][weatherTween.Type.Parameter].Kill();
            Tween tween = _entityTweeners[weatherTween.Type.Entity].Apply(weatherTween);
            _currentTweens[weatherTween.Type.Entity][weatherTween.Type.Parameter] = tween;
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
        
        public void Stop(WeatherEntityType entityType, WeatherParameterType parameterType)
        {
            if(!_currentTweens.ContainsKey(entityType) || !_currentTweens[entityType].ContainsKey(parameterType)) return;
            _currentTweens[entityType][parameterType].Kill();
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