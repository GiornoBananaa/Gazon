using System;
using Game.Runtime.PlayerInteractionSystem;
using Game.Runtime.PlayerMovementSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Runtime.InputFeature
{
    public class PlayerInputListener : IInputListener, IDisposable
    {
        private readonly IPlayerMovement _playerMovement;
        private readonly IPlayerInteraction _playerInteraction;
        private GameInputActions.PlayerActions _playerActions;
        
        public PlayerInputListener(GameInputActions actionMap, IPlayerMovement playerMovement, IPlayerInteraction playerInteraction)
        {
            _playerMovement = playerMovement;
            _playerInteraction = playerInteraction;
            _playerActions = actionMap.Player;
            EnableInput();
            _playerActions.Move.performed += OnMove;
            _playerActions.Move.canceled += OnMoveCanceled;
            _playerActions.Interact.performed += OnInteract;
        }
        
        public void Dispose()
        {
            if(_playerActions.Move == null) return;
            _playerActions.Move.performed -= OnMove;
            _playerActions.Move.canceled -= OnMoveCanceled;
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
        
        private void OnInteract(InputAction.CallbackContext callbackContext)
        {
            _playerInteraction.TryInteract();
        }
    }
}