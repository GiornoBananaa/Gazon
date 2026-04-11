using System;
using UnityEngine.InputSystem;

namespace Game.Runtime.InputFeature
{
    public class GeneralNavigationInputListener : IInputListener, IDisposable
    {
        private GameInputActions.NavigationActions _navigationActions;

        public event Action Exit;
        public event Action Back;
        public event Action KeyCanceled;
        
        public GeneralNavigationInputListener(GameInputActions actionMap)
        {
            _navigationActions = actionMap.Navigation;
            EnableInput();

            _navigationActions.Exit.performed += OnExit;
            _navigationActions.Back.performed += OnBack;
            _navigationActions.Submit.performed += OnSubmit;
        }
        
        public void Dispose()
        {
            if(_navigationActions.Exit == null) return;
            _navigationActions.Exit.performed -= OnExit;
            _navigationActions.Back.performed -= OnBack;
            _navigationActions.Submit.performed -= OnSubmit;
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
    }
}