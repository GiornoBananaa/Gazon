using DG.Tweening;
using UnityEngine;

namespace Game.Runtime.CameraSystem
{
    public class CameraInputRotator : ICameraRotator
    {
        private readonly Transform _xRotationPivot;
        private readonly Transform _yRotationPivot;
        private readonly float _inputSensitivity;
        private readonly float _animationTimePer180;
        private float _xRotation;
        private bool _enabled = true;
        private bool _animation;
        
        private Transform _forwardLimitTransform;
        private float _rotationLimitY;
        private float _rotationLimitX;
        private Tween _tween;
        
        public CameraInputRotator(CameraHandle cameraHandle)
        {
            _xRotationPivot = cameraHandle.XRotationPivot;
            _yRotationPivot = cameraHandle.YRotationPivot;
            _inputSensitivity = cameraHandle.Config.InputSensitivity;
            _animationTimePer180 = 0.5f;
        }

        public void Enable()
        {
            _enabled = true;
        }
        
        public void Disable()
        {
            _enabled = false;
        }
        
        public void SetLimitY(Transform forwardLimitTransform, float rotationLimitY, float rotationLimitX)
        {
            _forwardLimitTransform = forwardLimitTransform;
            _rotationLimitY = rotationLimitY;
            _rotationLimitX = rotationLimitX;

            float angleY = Vector3.SignedAngle(_forwardLimitTransform.forward, _yRotationPivot.forward, Vector3.up);
            float angleX = Vector3.SignedAngle(_forwardLimitTransform.forward, _xRotationPivot.forward, Vector3.right);
            
            if(Mathf.Abs(angleY) > _rotationLimitY || Mathf.Abs(angleX) > _rotationLimitX)
            {
                _tween?.Kill();
                var sequence = DOTween.Sequence();
                
                if (angleX > _rotationLimitX)
                    sequence.Join(_xRotationPivot.DOLocalRotate((_forwardLimitTransform.rotation.eulerAngles.x + _rotationLimitX) * Vector3.right, _animationTimePer180 * ((angleX - _rotationLimitX) / 180)));
                else if (angleX < -_rotationLimitX)
                    sequence.Join(_xRotationPivot.DOLocalRotate((_forwardLimitTransform.rotation.eulerAngles.x + -_rotationLimitX) * Vector3.right, _animationTimePer180 * ((_rotationLimitX - angleX) / 180)));
                if (angleY > _rotationLimitY)
                    sequence.Join(_yRotationPivot.DOLocalRotate((_forwardLimitTransform.rotation.eulerAngles.y + _rotationLimitY) * Vector3.up, _animationTimePer180 * ((angleY - _rotationLimitY) / 180)));
                else if (angleY < -_rotationLimitY)
                    sequence.Join(_yRotationPivot.DOLocalRotate((_forwardLimitTransform.rotation.eulerAngles.y + -_rotationLimitY) * Vector3.up, _animationTimePer180 * ((_rotationLimitY - angleY) / 180)));
                
                
                sequence.OnComplete(() => _animation = false);

                _animation = true;
                _tween = sequence;
            }
        }
        
        public void RemoveLimit()
        {
            _forwardLimitTransform = null;
        }
        
        public void InputLook(Vector2 lookInput)
        {
            if(!_enabled || _animation) return;
            float mouseX = lookInput.x * _inputSensitivity * Time.deltaTime;
            float mouseY = lookInput.y * _inputSensitivity * Time.deltaTime;
            
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
            
            _xRotationPivot.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            _yRotationPivot.Rotate(Vector3.up * mouseX);
            
            if(_forwardLimitTransform != null)
            {
                float angleY = Vector3.SignedAngle(_forwardLimitTransform.forward, _yRotationPivot.forward, Vector3.up);
                float angleX = Vector3.SignedAngle(_forwardLimitTransform.forward, _xRotationPivot.forward, Vector3.right);
                
                if (angleY > _rotationLimitY)
                    _yRotationPivot.rotation = Quaternion.AngleAxis(_forwardLimitTransform.rotation.eulerAngles.y + _rotationLimitY, Vector3.up);
                if (angleY < -_rotationLimitY)
                    _yRotationPivot.rotation = Quaternion.AngleAxis(_forwardLimitTransform.rotation.eulerAngles.y + -_rotationLimitY, Vector3.up);
                if (angleX > _rotationLimitX)
                {
                    _xRotationPivot.rotation = Quaternion.AngleAxis(_forwardLimitTransform.rotation.eulerAngles.x + _rotationLimitX, Vector3.right);
                    _xRotation = _xRotationPivot.localRotation.eulerAngles.x;
                }
                if (angleX < -_rotationLimitX)
                {
                    _xRotationPivot.rotation = Quaternion.AngleAxis(_forwardLimitTransform.rotation.eulerAngles.x + -_rotationLimitX, Vector3.right);
                    _xRotation = _xRotationPivot.localRotation.eulerAngles.x;
                }
            }
        }
    }
}