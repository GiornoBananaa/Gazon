using System;
using Game.Runtime.PlayerInteractionSystem;
using UnityEngine.InputSystem;

namespace Game.Runtime.InputFeature
{
    public class GeneralNavigationInputListener : IInputListener, IDisposable
    {
        private GameInputActions.NavigationActions _navigationActions;
        private readonly IPlayerInteraction _playerInteraction;
        
        public event Action Exit;
        public event Action Back;
        public event Action KeyCanceled;
        
        public GeneralNavigationInputListener(GameInputActions actionMap, IPlayerInteraction playerInteraction)
        {
            _navigationActions = actionMap.Navigation;
            _playerInteraction = playerInteraction;
            EnableInput();

            _navigationActions.Exit.performed += OnExit;
            _navigationActions.Back.performed += OnBack;
            _navigationActions.Submit.performed += OnSubmit;
            _navigationActions.ContinuousInteraction.performed += OnStartContinuousInteraction;
            _navigationActions.ContinuousInteraction.canceled += OnEndContinuousInteraction;
        }
        
        public void Dispose()
        {
            if(_navigationActions.Exit == null) return;
            _navigationActions.Exit.performed -= OnExit;
            _navigationActions.Back.performed -= OnBack;
            _navigationActions.Submit.performed -= OnSubmit;
            _navigationActions.ContinuousInteraction.performed -= OnStartContinuousInteraction;
            _navigationActions.ContinuousInteraction.canceled -= OnEndContinuousInteraction;
        }
        
        public void EnableInput()
        {
            _navigationActions.Enable();
        }
    
        public void DisableInput()
        {
            _navigationActions.Disable();
        }

        private void OnExit(InputAction.CallbackContext callbackContext)
        {
            Exit?.Invoke();
        }
        
        private void OnBack(InputAction.CallbackContext callbackContext)
        {
            Back?.Invoke();
        }
        
        private void OnSubmit(InputAction.CallbackContext callbackContext)
        {
            KeyCanceled?.Invoke();
        }
        
        private void OnStartContinuousInteraction(InputAction.CallbackContext callbackContext)
        {
            _playerInteraction.TryStartContinuousInteraction();
        }
        
        private void OnEndContinuousInteraction(InputAction.CallbackContext callbackContext)
        {
            _playerInteraction.EndContinuousInteraction();
        }
    }
}