using System.Collections.Generic;
using Game.Runtime.CameraSystem;
using Game.Runtime.ServiceSystem;
using UnityEngine;

namespace Game.Runtime.ObjectCullingSystem
{
    public class ObjectCuller : IUpdatable
    {
        private readonly ICurrentCamera _currentCamera;
        private readonly HashSet<ObjectCullingTarget> _observedObjects;
        
        public ObjectCuller(ICurrentCamera currentCamera)
        {
            _currentCamera = currentCamera;
            _observedObjects = new HashSet<ObjectCullingTarget>();
        }

        public void Subscribe(ObjectCullingTarget observable)
        {
            _observedObjects.Add(observable);
        }
        
        public void Unsubscribe(ObjectCullingTarget observable)
        {
            if(_observedObjects.Contains(observable))
                _observedObjects.Remove(observable);
        }

        public void Update(float deltaTime)
        {
            Camera camera = _currentCamera.GetCurrentCamera();
            
            if(camera == null) return;
            
            foreach (var target in _observedObjects)
            {
                bool isVisible = true;
                foreach (IObjectCullingRule rule in target.CullingRules)
                {
                    if (rule.IsCullObject(camera, target)) continue;
                    isVisible = false;
                    break;
                }
                
                target.IsCulled.Value = isVisible;
            }
        }
    }
}