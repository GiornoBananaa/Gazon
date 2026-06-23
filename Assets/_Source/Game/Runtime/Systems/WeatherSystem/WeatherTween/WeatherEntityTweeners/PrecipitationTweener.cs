using DG.Tweening;

namespace Game.Runtime.WeatherSystem.WeatherTween.WeatherEntityTweeners
{
    public class PrecipitationTweener : WeatherEntityTweener
    {
        public override WeatherEntityType EntityType => WeatherEntityType.Precipitation;
        
        private readonly ITweenTarget<float> _forceTarget;
        
        public PrecipitationTweener(PrecipitationController windPositionsMover)
        {
            _forceTarget = new PrecipitationForceTarget(windPositionsMover);
        }

        protected override Tween TweenForce(WeatherTween tween) => tween.ApplyTo(_forceTarget);
        
        private class PrecipitationForceTarget : ITweenTarget<float>
        {
            private readonly PrecipitationController _controller;
            public PrecipitationForceTarget(PrecipitationController controller) => _controller = controller;
            
            public float GetTweenValue() => _controller.Precipitation;
            public void SetTweenValue(float value) => _controller.SetPrecipitation(value);
        }
    }
}