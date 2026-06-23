using System;
using Game.Runtime.CameraSystem;
using Game.Runtime.Utils;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.PlanetFeature
{
    public class RoundWorldPositionSetter : MonoBehaviour
    {
        private ICurrentCamera _currentCamera;
        private Vector3 _localPosition;
        
        [Inject]
        public void Construct(ICurrentCamera currentCamera)
        {
            _currentCamera = currentCamera;
        }

        private void Awake()
        {
            _localPosition = transform.localPosition;
        }

        private void Update()
        {
            if(transform.parent == null) return;
            transform.position = MathUtils.ConvertToRoundWorldPosition(transform.parent.TransformPoint(_localPosition),
                _currentCamera.GetCurrentCamera().transform.position);
        }
    }
}