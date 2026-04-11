using System;
using Game.Runtime.CameraSystem;
using Game.Runtime.PlayerMovementSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Runtime.InputFeature
{
    public class CameraInputListener : IInputListener, IDisposable
    {
        private readonly ICameraRotator _cameraInputRotator;
        private readonly IPlayerMovement _playerMovement;
        private GameInputActions.CameraActions _cameraActions;
        
        public CameraInputListener(GameInputActions actionMap, ICameraRotator cameraInputRotator)
        {
            _cameraInputRotator = cameraInputRotator;
            _cameraActions = actionMap.Camera;
            EnableInput();

            _cameraActions.Look.performed += OnLook;
        }
        
        public void Dispose()
        {
            if(_cameraActions.Look == null) return;
            _cameraActions.Look.performed -= OnLook;
        }
        
        public void EnableInput() => _cameraActions.Enable();
        
        public void DisableInput() => _cameraActions.Disable();
        
        private void OnLook(InputAction.CallbackContext callbackContext)
        {
            _cameraInputRotator.InputLook(callbackContext.ReadValue<Vector2>());
        }
    }
}