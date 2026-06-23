using Game.Runtime.Configs;
using Game.Runtime.ServiceSystem;
using UnityEngine;

namespace Game.Runtime.WeatherSystem.WeatherTween.WeatherEntityTweeners
{
    public class WindPositionMover : IUpdatable
    {
        private readonly int _windPositionProperty = Shader.PropertyToID("_WindPosition");
        private readonly int _windRotationProperty = Shader.PropertyToID("_Rotation");
        private Material[] _materials;
        
        private Vector2 _windPosition;
        private float _angleInDegrees = 135;

        public float WindSpeed { get; private set; } = 6f;
        public Vector2 WindDirection { get; private set; } = Vector2.up;

        public WindPositionMover()
        {
            _materials = RootConfig.Instance.WeatherAssets.GrassMaterial;
        }

        public void SetWindSpeed(float speed) => WindSpeed = speed;
        
        public void SetWindDirection(Vector2 direction) => WindDirection = direction.normalized;
        
        public void Update()
        {
            if(_materials == null) return;
            _windPosition += WindDirection * (WindSpeed * Time.deltaTime);
            float angleInRadians = Mathf.Atan2(WindDirection.x, WindDirection.y);
            _angleInDegrees = Mathf.LerpAngle(_angleInDegrees, 135 + angleInRadians * Mathf.Rad2Deg,2 * Time.deltaTime);
            
            foreach (var material in _materials)
            {
                material.SetVector(_windPositionProperty, -_windPosition);
                material.SetFloat(_windRotationProperty, _angleInDegrees);
            }
        }
    }
}