using UnityEngine;

namespace Game.Runtime.WeatherSystem.WeatherTween
{
    public abstract class VFXPropertyTarget<T> : ITweenTarget<T>
    {
        protected readonly int Property;
        protected readonly Material VFX;

        public VFXPropertyTarget(Material vfx, string property)
        {
            this.Property = Shader.PropertyToID(property);
            VFX = vfx;
        }
        public abstract T GetTweenValue();
        public abstract void SetTweenValue(T value);
    }
    
    public class VFXFloatTarget : VFXPropertyTarget<float>
    {
        public VFXFloatTarget(Material vfx, string property) : base(vfx, property) { }

        public override float GetTweenValue() => VFX.GetFloat(Property);
        public override void SetTweenValue(float value) => VFX.SetFloat(Property, value);
    }
}