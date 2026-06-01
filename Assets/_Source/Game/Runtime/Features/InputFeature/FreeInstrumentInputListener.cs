using Game.Runtime.PianoFeature;
using UnityEngine.InputSystem;

namespace Game.Runtime.InputFeature
{
    public class FreeInstrumentInputListener : IInputListener
    {
        private readonly InputAction[] _keyActions;
        private readonly InputAction[] _noteActions;
        private readonly FreeInstrumentKeyPresser _instrumentKeyPresser;
        private GameInputActions.PianoActions _pianoActions;

        private bool _enabled = true;
        
        public FreeInstrumentInputListener(GameInputActions actionMap, FreeInstrumentKeyPresser instrumentKeyPresser)
        {
            _pianoActions = actionMap.Piano;
            _instrumentKeyPresser = instrumentKeyPresser;
            
            _keyActions = new[]
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

            _noteActions = new[]
            {
                _pianoActions.NoteA0,
                _pianoActions.NoteAS0,
                _pianoActions.NoteB0,
                
                _pianoActions.NoteC1,
                _pianoActions.NoteCS1,
                _pianoActions.NoteD1,
                _pianoActions.NoteDS1,
                _pianoActions.NoteE1,
                _pianoActions.NoteF1,
                _pianoActions.NoteFS1,
                _pianoActions.NoteG1,
                _pianoActions.NoteGS1,
                _pianoActions.NoteA1,
                _pianoActions.NoteAS1,
                _pianoActions.NoteB1,
                
                _pianoActions.NoteC2,
                _pianoActions.NoteCS2,
                _pianoActions.NoteD2,
                _pianoActions.NoteDS2,
                _pianoActions.NoteE2,
                _pianoActions.NoteF2,
                _pianoActions.NoteFS2,
                _pianoActions.NoteG2,
                _pianoActions.NoteGS2,
                _pianoActions.NoteA2,
                _pianoActions.NoteAS2,
                _pianoActions.NoteB2,
                
                _pianoActions.NoteC3,
                _pianoActions.NoteCS3,
                _pianoActions.NoteD3,
                _pianoActions.NoteDS3,
                _pianoActions.NoteE3,
                _pianoActions.NoteF3,
                _pianoActions.NoteFS3,
                _pianoActions.NoteG3,
                _pianoActions.NoteGS3,
                _pianoActions.NoteA3,
                _pianoActions.NoteAS3,
                _pianoActions.NoteB3,
                
                _pianoActions.NoteC4,
                _pianoActions.NoteCS4,
                _pianoActions.NoteD4,
                _pianoActions.NoteDS4,
                _pianoActions.NoteE4,
                _pianoActions.NoteF4,
                _pianoActions.NoteFS4,
                _pianoActions.NoteG4,
                _pianoActions.NoteGS4,
                _pianoActions.NoteA4,
                _pianoActions.NoteAS4,
                _pianoActions.NoteB4,
                
                _pianoActions.NoteC5,
                _pianoActions.NoteCS5,
                _pianoActions.NoteD5,
                _pianoActions.NoteDS5,
                _pianoActions.NoteE5,
                _pianoActions.NoteF5,
                _pianoActions.NoteFS5,
                _pianoActions.NoteG5,
                _pianoActions.NoteGS5,
                _pianoActions.NoteA5,
                _pianoActions.NoteAS5,
                _pianoActions.NoteB5,
                
                _pianoActions.NoteC6,
                _pianoActions.NoteCS6,
                _pianoActions.NoteD6,
                _pianoActions.NoteDS6,
                _pianoActions.NoteE6,
                _pianoActions.NoteF6,
                _pianoActions.NoteFS6,
                _pianoActions.NoteG6,
                _pianoActions.NoteGS6,
                _pianoActions.NoteA6,
                _pianoActions.NoteAS6,
                _pianoActions.NoteB6,
                
                _pianoActions.NoteC7,
                _pianoActions.NoteCS7,
                _pianoActions.NoteD7,
                _pianoActions.NoteDS7,
                _pianoActions.NoteE7,
                _pianoActions.NoteF7,
                _pianoActions.NoteFS7,
                _pianoActions.NoteG7,
                _pianoActions.NoteGS7,
                _pianoActions.NoteA7,
                _pianoActions.NoteAS7,
                _pianoActions.NoteB7,
                _pianoActions.NoteC8,
            };
            for (int i = 0; i < _keyActions.Length; i++)
            {
                int keyIndex = i;
                _keyActions[i].started += c => OnKeyStarted(c, keyIndex);
                _keyActions[i].canceled += c => OnKeyCanceled(c, keyIndex);
            }
            for (int i = 0; i < _noteActions.Length; i++)
            {
                int keyIndex = i;
                _noteActions[i].started += c => OnNoteStarted(c, keyIndex);
                _noteActions[i].canceled += c => OnNoteCanceled(c, keyIndex);
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

        private void OnNoteStarted(InputAction.CallbackContext context, int keyNumber)
        {
            if(_instrumentKeyPresser.KeysCount != _noteActions.Length)
                _instrumentKeyPresser.SetKeysCount(_noteActions.Length);
            _instrumentKeyPresser.PressKey(keyNumber);
        }
        
        private void OnNoteCanceled(InputAction.CallbackContext context, int keyNumber)
        {
            _instrumentKeyPresser.ReleaseKey(keyNumber);
        }
        
        private void OnKeyStarted(InputAction.CallbackContext context, int keyNumber)
        {
            if(_instrumentKeyPresser.KeysCount != _keyActions.Length)
                _instrumentKeyPresser.SetKeysCount(_keyActions.Length);
            _instrumentKeyPresser.PressKey(keyNumber);
        }
        
        private void OnKeyCanceled(InputAction.CallbackContext context, int keyNumber)
        {
            _instrumentKeyPresser.ReleaseKey(keyNumber);
        }
        
        private void OnOctaveUp(InputAction.CallbackContext context)
        {
            _instrumentKeyPresser.OctaveUp();
        }
        
        private void OnOctaveDown(InputAction.CallbackContext context)
        {
            _instrumentKeyPresser.OctaveDown();
        }
        
        private void OnPressPedal(InputAction.CallbackContext context)
        {
            _instrumentKeyPresser.PressPedal();
        }
        
        private void OnReleasePedal(InputAction.CallbackContext context)
        {
            _instrumentKeyPresser.ReleasePedal();
        }
    }
}