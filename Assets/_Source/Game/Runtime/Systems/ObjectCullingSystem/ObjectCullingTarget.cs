using R3;
using UnityEngine;

namespace Game.Runtime.ObjectCullingSystem
{
    public class ObjectCullingTarget
    {
        public readonly Transform Transform;
        public readonly Bounds LocalBounds;
        public readonly IObjectCullingRule[] CullingRules;
        
        public readonly ReactiveProperty<bool> IsCulled = new();
        
        public ObjectCullingTarget(Transform transform, Bounds localBounds, IObjectCullingRule[] cullingRules)
        {
            Transform = transform;
            LocalBounds = localBounds;
            CullingRules = cullingRules;
        }
    }
}