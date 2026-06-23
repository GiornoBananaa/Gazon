using DG.Tweening;

namespace Game.Runtime.WeatherSystem.WeatherTween.WeatherEntityTweeners
{
    public class TemperatureTweener : WeatherEntityTweener
    {
        public override WeatherEntityType EntityType => WeatherEntityType.Temperature;
        
        private readonly ITweenTarget<float> _forceTarget;
        
        public TemperatureTweener(PrecipitationController windPositionsMover)
        {
            _forceTarget = new ForceTarget(windPositionsMover);
        }

        protected override Tween TweenForce(WeatherTween tween) => tween.ApplyTo(_forceTarget);
        
        private class ForceTarget : ITweenTarget<float>
        {
            private readonly PrecipitationController _controller;
            public ForceTarget(PrecipitationController controller) => _controller = controller;
            
            public float GetTweenValue() => _controller.Temperature;
            public void SetTweenValue(float value) => _controller.SetTemperature(value);
        }
    }
}