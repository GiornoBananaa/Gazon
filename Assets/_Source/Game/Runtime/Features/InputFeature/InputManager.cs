using System.Collections.Generic;
using Reflex.Attributes;

namespace Game.Runtime.InputFeature
{
    public class InputManager
    {
        private GameInputActions _gameInput;

        [Inject]
        public void Construct(GameInputActions gameInputActions, IEnumerable<IInputListener> inputListeners)
        {
            _gameInput = gameInputActions;
            EnableInput();
        }

        public void EnableInput()
        {
            if(_gameInput == null) 
                _gameInput = new GameInputActions();
            _gameInput.Enable();
        }
    
        public void DisableInput()
        {
            _gameInput?.Disable();
        }
    }
}