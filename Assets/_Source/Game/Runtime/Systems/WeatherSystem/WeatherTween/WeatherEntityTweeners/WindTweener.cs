using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.Runtime.Configs;
using UnityEngine;

namespace Game.Runtime.WeatherSystem.WeatherTween.Tweeners
{
    public class WindTweener : WeatherEntityTweener
    {
        public override WeatherEntityType EntityType => WeatherEntityType.Wind;

        private readonly IEnumerable<ITweenTarget<float>> _grassBendForceTarget;
        private readonly ITweenTarget<float> _windSpeedTarget;
        private readonly ITweenTarget<Quaternion> _windAngleTarget;
        private readonly float _minSpeed;
        private readonly float _maxSpeed;
        private readonly WeatherTween _speedTween = new();
        
        public WindTweener(WindPositionMover windPositionsMover, WeatherPianoBindConfig weatherPianoBindConfig)
        {
            Material[] materials = RootConfig.Instance.WeatherAssets.GrassMaterial;
            _grassBendForceTarget = materials.Select(m => new MaterialFloatTarget(m, "_BendStrength"));
            _windSpeedTarget = new WindSpeedTarget(windPositionsMover);
            _windAngleTarget = new WindDirectionTarget(windPositionsMover);
            _minSpeed = weatherPianoBindConfig.MinSpeed;
            _maxSpeed = weatherPianoBindConfig.MaxSpeed;
        }

        protected override Tween TweenForce(WeatherTween tween)
        {
            var sequence = GetSequence(tween, _grassBendForceTarget);
            _speedTween.Type = tween.Type;
            _speedTween.TweenConfig = tween.TweenConfig;
            _speedTween.FloatValue = Mathf.Lerp(_minSpeed, _maxSpeed, tween.FloatValue);
            return sequence.Join(_speedTween.ApplyTo(_windSpeedTarget));
        }

        protected override Tween TweenDirection(WeatherTween tween) => tween.ApplyTo(_windAngleTarget);
        
        private class WindSpeedTarget : ITweenTarget<float>
        {
            private readonly WindPositionMover _mover;
            public WindSpeedTarget(WindPositionMover mover) => _mover = mover;
            
            public float GetTweenValue() => _mover.WindSpeed;
            public void SetTweenValue(float value) => _mover.SetWindSpeed(value);
        }
        
        private class WindDirectionTarget : ITweenTarget<Quaternion>
        {
            private readonly WindPositionMover _mover;
            private Quaternion _windDirection;
            
            public WindDirectionTarget(WindPositionMover mover)
            {
                _mover = mover;
            }

            public Quaternion GetTweenValue() => _windDirection;

            public void SetTweenValue(Quaternion value)
            {
                _windDirection = value; 
                _mover.SetWindDirection(Quaternion.Inverse(value) * Vector2.up);
            }
        }
    }
}