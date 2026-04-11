using Game.Runtime.CameraFeature;
using UnityEngine;

namespace Game.Runtime.CameraSystem
{
    public class CameraInputRotator : ICameraRotator
    {
        private readonly Transform _xRotationPivot;
        private readonly Transform _yRotationPivot;
        private readonly float _inputSensitivity;
        private float _xRotation;
        private bool _enabled = true;
        
        private Transform _forwardLimitYTransform;
        private float _rotationLimitY;
        
        public CameraInputRotator(CameraHandle cameraHandle)
        {
            _xRotationPivot = cameraHandle.XRotationPivot;
            _yRotationPivot = cameraHandle.YRotationPivot;
            _inputSensitivity = cameraHandle.Config.InputSensitivity;
        }

        public void Enable()
        {
            _enabled = true;
        }
        
        public void Disable()
        {
            _enabled = false;
        }
        
        public void SetLimitY(Transform forwardLimitTransform, float rotationLimitY)
        {
            _forwardLimitYTransform = forwardLimitTransform;
            _rotationLimitY = rotationLimitY;
        }
        
        public void RemoveLimit()
        {
            _forwardLimitYTransform = null;
        }
        
        public void InputLook(Vector2 lookInput)
        {
            if(!_enabled) return;
            float mouseX = lookInput.x * _inputSensitivity * Time.deltaTime;
            float mouseY = lookInput.y * _inputSensitivity * Time.deltaTime;
            
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
            
            _xRotationPivot.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            _yRotationPivot.Rotate(Vector3.up * mouseX);
            
            if(_forwardLimitYTransform != null)
            {
                float angle = Mathf.Abs(Vector3.SignedAngle(_forwardLimitYTransform.forward, _yRotationPivot.forward, Vector3.up));
                
                if (angle > _rotationLimitY)
                    _yRotationPivot.rotation = Quaternion.RotateTowards(_forwardLimitYTransform.rotation, _yRotationPivot.rotation, _rotationLimitY);
            }
        }
    }
}