using UnityEngine;

namespace Game.Runtime.WeatherSystem.WeatherTween
{
    public abstract class MaterialPropertyTarget<T> : ITweenTarget<T>
    {
        protected readonly int Property;
        protected readonly Material Material;

        public MaterialPropertyTarget(Material material, string property)
        {
            this.Property = Shader.PropertyToID(property);
            Material = material;
        }
        public abstract T GetWeatherTweenValue();
        public abstract void SetWeatherTweenValue(T value);
    }
    
    public class MaterialFloatTarget : MaterialPropertyTarget<float>
    {
        public MaterialFloatTarget(Material material, string property) : base(material, property) { }

        public override float GetWeatherTweenValue() => Material.GetFloat(Property);
        public override void SetWeatherTweenValue(float value) => Material.SetFloat(Property, value);
    }
    
    public class MaterialVectorTarget : MaterialPropertyTarget<Vector2>
    {
        public MaterialVectorTarget(Material material, string property) : base(material, property) { }

        public override Vector2 GetWeatherTweenValue() => Material.GetVector(Property);
        public override void SetWeatherTweenValue(Vector2 value) => Material.SetVector(Property, value);
    }
}