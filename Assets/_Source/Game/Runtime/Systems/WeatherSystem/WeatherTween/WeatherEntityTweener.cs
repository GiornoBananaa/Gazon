using System;
using System.Collections.Generic;
using DG.Tweening;

namespace Game.Runtime.WeatherSystem.WeatherTween
{
    public abstract class WeatherEntityTweener
    {
        public Tween Apply(WeatherTween tween)
        {
            switch (tween.Type.Parameter)
            {
                case WeatherParameterType.Force: return TweenForce(tween);
                case WeatherParameterType.Direction: return TweenDirection(tween);
                case WeatherParameterType.Position: return TweenPosition(tween);
                case WeatherParameterType.Color: return TweenColor(tween);
            }
            throw new ArgumentException("Invalid weather property type");
        }

        protected virtual Tween TweenForce(WeatherTween tween) => throw new NotImplementedException($"Force tween for {tween.Type.Entity} is not implemented");
        protected virtual Tween TweenDirection(WeatherTween tween) => throw new NotImplementedException($"Direction tween for {tween.Type.Entity} is not implemented");
        protected virtual Tween TweenPosition(WeatherTween tween) => throw new NotImplementedException($"Position tween for {tween.Type.Entity} is not implemented");
        protected virtual Tween TweenColor(WeatherTween tween) => throw new NotImplementedException($"Color tween for {tween.Type.Entity} is not implemented");
        
        protected Tween GetSequence<T>(WeatherTween tween, IEnumerable<ITweenTarget<T>> targets)
        {
            var sequence = DOTween.Sequence();
            foreach (var target in targets)
            {
                sequence.Join(tween.ApplyTo(target));
            }
            
            return sequence;
        }
    }
}