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
        public abstract T GetTweenValue();
        public abstract void SetTweenValue(T value);
    }
    
    public class MaterialFloatTarget : MaterialPropertyTarget<float>
    {
        public MaterialFloatTarget(Material material, string property) : base(material, property) { }

        public override float GetTweenValue() => Material.GetFloat(Property);
        public override void SetTweenValue(float value) => Material.SetFloat(Property, value);
    }
    
    public class MaterialVectorTarget : MaterialPropertyTarget<Vector2>
    {
        public MaterialVectorTarget(Material material, string property) : base(material, property) { }

        public override Vector2 GetTweenValue() => Material.GetVector(Property);
        public override void SetTweenValue(Vector2 value) => Material.SetVector(Property, value);
    }
}