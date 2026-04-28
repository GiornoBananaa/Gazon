using Game.Runtime.PianoRhythmSystem;
using UnityEngine.InputSystem;

namespace Game.Runtime.InputFeature
{
    public class PianoInputListener : IInputListener
    {
        private readonly InputAction[] _noteActions;
        private readonly PianoKeysPressFacade _keysPresser;
        private GameInputActions.PianoActions _pianoActions;

        private bool _enabled = true;
        
        public PianoInputListener(GameInputActions actionMap, PianoKeysPressFacade keysPresser)
        {
            _pianoActions = actionMap.Piano;
            _keysPresser = keysPresser;
            
            _noteActions = new[]
            {
                _pianoActions.Key1,
                _pianoActions.BlackKey1,
                _pianoActions.Key2,
                _pianoActions.BlackKey2,
                _pianoActions.Key3,
                _pianoActions.Key4,
                _pianoActions.BlackKey3,
                _pianoActions.Key5,
                _pianoActions.BlackKey4,
                _pianoActions.Key6,
                _pianoActions.BlackKey5,
                _pianoActions.Key7,
                _pianoActions.Key8,
                _pianoActions.BlackKey6,
                _pianoActions.Key9,
                _pianoActions.BlackKey7,
                _pianoActions.Key10,
                _pianoActions.Key11,
                _pianoActions.BlackKey8,
                _pianoActions.Key12,
                _pianoActions.BlackKey9,
                _pianoActions.Key13,
                _pianoActions.BlackKey10,
                _pianoActions.Key14
            };
            
            for (int i = 0; i < _noteActions.Length; i++)
            {
                int keyIndex = i;
                _noteActions[i].started += c => OnKeyStarted(c, keyIndex);
                _noteActions[i].canceled += c => OnKeyCanceled(c, keyIndex);
            }
            
            DisableInput();
        }
        
        public void EnableInput()
        {
            _pianoActions.Enable();
            if(_enabled) return;
            _pianoActions.OctaveUp.performed += OnOctaveUp;
            _pianoActions.OctaveDown.performed += OnOctaveDown;
            _pianoActions.Pedal.started += OnPressPedal;
            _pianoActions.Pedal.canceled += OnReleasePedal;
            _enabled = true;
        }
    
        public void DisableInput()
        {
            _pianoActions.Disable();
            if(!_enabled) return;
            _pianoActions.OctaveUp.performed -= OnOctaveUp;
            _pianoActions.OctaveDown.performed -= OnOctaveDown;
            _pianoActions.Pedal.started -= OnPressPedal;
            _pianoActions.Pedal.canceled -= OnReleasePedal;
            _enabled = false;
        }

        private void OnKeyStarted(InputAction.CallbackContext context, int keyNumber)
        {
            _keysPresser.PressKey(keyNumber);
        }
        
        private void OnKeyCanceled(InputAction.CallbackContext context, int keyNumber)
        {
            _keysPresser.ReleaseKey(keyNumber);
        }
        
        private void OnOctaveUp(InputAction.CallbackContext context)
        {
            _keysPresser.OctaveUp();
        }
        
        private void OnOctaveDown(InputAction.CallbackContext context)
        {
            _keysPresser.OctaveDown();
        }
        
        private void OnPressPedal(InputAction.CallbackContext context)
        {
            _keysPresser.PressPedal();
        }
        
        private void OnReleasePedal(InputAction.CallbackContext context)
        {
            _keysPresser.ReleasePedal();
        }
    }
}