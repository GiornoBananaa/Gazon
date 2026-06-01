using System;
using Game.Runtime.CameraSystem;
using Game.Runtime.PlayerInteractionSystem;
using Game.Runtime.PlayerMovementSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Runtime.InputFeature
{
    public class CameraInputListener : IInputListener, IDisposable
    {
        private readonly ICameraRotator _cameraInputRotator;
        private readonly IPlayerMovement _playerMovement;
        private readonly IPlayerInteraction _playerInteraction;
        private GameInputActions.CameraActions _cameraActions;
        
        public CameraInputListener(GameInputActions actionMap, ICameraRotator cameraInputRotator, IPlayerInteraction playerInteraction)
        {
            _cameraInputRotator = cameraInputRotator;
            _cameraActions = actionMap.Camera;
            _playerInteraction = playerInteraction;
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
            var value = callbackContext.ReadValue<Vector2>();
            _cameraInputRotator.InputLook(value);
            _playerInteraction.OnLookChanged(value);
        }
    }
}