using UnityEngine;

namespace Game.Runtime.ObjectCullingSystem.CullingRules
{
    public class DistanceCullingRule : IObjectCullingRule
    {
        private readonly float _distance;
        
        public DistanceCullingRule(float distance)
        {
            _distance = distance;
        }
        
        public bool IsCullObject(Camera camera, ObjectCullingTarget target)
        {
            return Vector3.Distance(camera.transform.position, target.Transform.transform.position) < _distance;
        }
    }
}