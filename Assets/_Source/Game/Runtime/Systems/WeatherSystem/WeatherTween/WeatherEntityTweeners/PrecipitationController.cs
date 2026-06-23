using System.Collections.Generic;
using Game.Runtime.CameraSystem;
using Game.Runtime.Configs;
using Game.Runtime.ServiceSystem;
using Game.Runtime.Utils;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.Runtime.WeatherSystem.WeatherTween.WeatherEntityTweeners
{
    public class PrecipitationController : IUpdatable
    {
        private const string DENSITY_PROPERTY = "Density";
        private const string FORCE_PROPERTY = "Force";
        private readonly WeatherPianoBindConfig _weatherConfig;
        private readonly WindPositionMover _windMover;
        private readonly ICurrentCamera _currentCamera;
        private readonly VisualEffect _rain;
        private readonly VisualEffect _snow;
        
        public float Precipitation { get; private set; }
        public float Temperature { get; private set; }
        
        public PrecipitationController(ICurrentCamera currentCamera, WindPositionMover windMover, WeatherPianoBindConfig weatherConfig)
        {
            _currentCamera = currentCamera;
            _weatherConfig = weatherConfig;
            _windMover = windMover;
            _rain = new GameObject("VFX_Rain").AddComponent<VisualEffect>();
            _snow = new GameObject("VFX_Snow").AddComponent<VisualEffect>();
            _rain.visualEffectAsset = weatherConfig.RainEffect;
            _snow.visualEffectAsset = weatherConfig.SnowEffect;
            SetPrecipitation(0);
            SetTemperature(1);
            UpdateForce();
        }
        
        public void SetPrecipitation(float precipitation)
        {
            precipitation = Mathf.Clamp(precipitation, 0, 1);
            Precipitation = precipitation;
            UpdateDensity();
        }
        
        public void SetTemperature(float temperature)
        {
            temperature = Mathf.Clamp(temperature, 0, 1);
            Temperature = temperature;
            UpdateDensity();
        }
        
        public void Update()
        {
            var followTransform = _currentCamera.GetCurrentCamera().transform;

            UpdateForce();
            
            _rain.transform.position = followTransform.position;
            _snow.transform.position = followTransform.position;
        }

        private void UpdateDensity()
        {
            _rain.SetInt(DENSITY_PROPERTY, (int)(Precipitation * Temperature * _weatherConfig.RainMaxDensity));
            _snow.SetInt(DENSITY_PROPERTY, (int)(Precipitation * (1 - Temperature) * _weatherConfig.SnowMaxDensity));
        }
        private void UpdateForce()
        {
            Vector3 wind = _windMover.WindDirection.GetVectorXZ();
            _rain.SetVector3(FORCE_PROPERTY, wind * (_windMover.WindSpeed * _weatherConfig.RainWindSpeedBlend) + Vector3.down * _weatherConfig.RainFallSpeed);
            _snow.SetVector3(FORCE_PROPERTY, wind * (_windMover.WindSpeed * _weatherConfig.SnowWindSpeedBlend) + Vector3.down * _weatherConfig.SnowFallSpeed);
        }
    }
}