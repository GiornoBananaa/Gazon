using System;
using Game.Runtime.CameraSystem;
using Game.Runtime.PlayerInteractionSystem;
using Game.Runtime.PlayerMovementSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Runtime.InputFeature
{
    public class PlayerInputListener : IInputListener, IDisposable
    {
        private readonly ICameraRotator _cameraInputRotator;
        private readonly IPlayerMovement _playerMovement;
        private readonly IPlayerInteraction _playerInteraction;
        private GameInputActions.PlayerActions _playerActions;
        
        public PlayerInputListener(GameInputActions actionMap, IPlayerMovement playerMovement,
            ICameraRotator cameraInputRotator, IPlayerInteraction playerInteraction)
        {
            _cameraInputRotator = cameraInputRotator;
            _playerMovement = playerMovement;
            _playerInteraction = playerInteraction;
            _playerActions = actionMap.Player;
            EnableInput();
            _playerActions.Move.performed += OnMove;
            _playerActions.Move.canceled += OnMoveCanceled;
            _playerActions.Look.performed += OnLook;
            _playerActions.Interact.performed += OnInteract;
        }
        
        public void Dispose()
        {
            if(_playerActions.Move == null) return;
            _playerActions.Move.performed -= OnMove;
            _playerActions.Move.canceled -= OnMoveCanceled;
            _playerActions.Look.performed -= OnLook;
            _playerActions.Interact.performed -= OnInteract;
        }
        
        public void EnableInput() => _playerActions.Enable();
    
        public void DisableInput() => _playerActions.Disable();
        
        private void OnMove(InputAction.CallbackContext callbackContext)
        {
            Vector3 direction = callbackContext.ReadValue<Vector2>();
            direction = new Vector3(direction.x, 0, direction.y);
            _playerMovement.SetMoveDirection(direction);
        }
        
        private void OnMoveCanceled(InputAction.CallbackContext callbackContext)
        {
            _playerMovement.SetMoveDirection(Vector3.zero);
        }
        
        private void OnLook(InputAction.CallbackContext callbackContext)
        {
            _cameraInputRotator.InputLook(callbackContext.ReadValue<Vector2>());
        }
        
        private void OnInteract(InputAction.CallbackContext callbackContext)
        {
            _playerInteraction.TryInteract();
        }
    }
}