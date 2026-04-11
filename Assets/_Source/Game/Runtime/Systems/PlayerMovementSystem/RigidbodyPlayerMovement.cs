using Game.Runtime.CameraFeature;
using Game.Runtime.CameraSystem;
using Game.Runtime.PlayerFeature;
using Game.Runtime.ServiceSystem;
using UnityEngine;

namespace Game.Runtime.PlayerMovementSystem
{
    public class RigidbodyPlayerMovement : IPlayerMovement, IFixedUpdatable
    {
        private readonly Rigidbody _rigidbody;
        private readonly CameraHandle _cameraHandle;
        private readonly float _speed;
        
        private bool _enabled = true;
        private Vector3 _localVelocity;

        public RigidbodyPlayerMovement(CameraHandle cameraHandleHandle, Player player)
        {
            _cameraHandle = cameraHandleHandle;
            _rigidbody = player.Rigidbody;
            _speed = player.Config.Speed;
        }

        public void Enable() => _enabled = true;

        public void Disable() => _enabled = false;

        public void SetMoveDirection(Vector3 directionNormalized)
        {
            _localVelocity = directionNormalized * _speed;
        }
        
        void IFixedUpdatable.FixedUpdate()
        {
            if(!_enabled) return;
            
            Vector3 move = _cameraHandle.YRotationPivot.TransformDirection(_localVelocity) * Time.fixedDeltaTime;
            move = new Vector3(move.x, 0, move.z);
            _rigidbody.MovePosition(_rigidbody.position + move);
        }
    }
}