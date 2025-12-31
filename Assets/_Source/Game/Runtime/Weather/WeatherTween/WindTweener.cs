using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.Runtime.Configs;
using UnityEngine;

namespace Game.Runtime.Weather.WeatherTween
{
    public class WindTweener : WeatherEntityTweener
    {
        private readonly IEnumerable<ITweenTarget<float>> _windForceTarget;
        private readonly IEnumerable<ITweenTarget<Vector2>> _windDirectionTarget;

        public WindTweener()
        {
            Material[] materials = RootConfig.Instance.WeatherAssets.GrassMaterial;
            _windForceTarget = materials.Select(m => new MaterialFloatTarget(m, "_BendStrength"));
            _windDirectionTarget = materials.Select(m => new MaterialVectorTarget(m, "_WindDirection"));
        }

        protected override Tween TweenForce(WeatherTween tween) => GetSequence(tween, _windForceTarget);

        protected override Tween TweenDirection(WeatherTween tween) => GetSequence(tween, _windDirectionTarget);
    }
}