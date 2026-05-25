using Game.Runtime.PianoFeature;
using UnityEngine.InputSystem;

namespace Game.Runtime.InputFeature
{
    public class RhythmInstrumentInputListener : IInputListener
    {
        private readonly InputAction[] _noteActions;
        private readonly IInstrumentKeyPresser _instrumentKeyPresser;
        private GameInputActions.PianoRhythmActions _pianoActions;

        private bool _enabled = true;
        
        public RhythmInstrumentInputListener(GameInputActions actionMap, RhythmGameInstrumentKeyPresser keysPresser)
        {
            _pianoActions = actionMap.PianoRhythm;
            _instrumentKeyPresser = keysPresser;
            
            _noteActions = new[]
            {
                _pianoActions.Key1,
                _pianoActions.Key2,
                _pianoActions.Key3,
                _pianoActions.Key4,
                _pianoActions.Key5,
                _pianoActions.Key6,
                _pianoActions.Key7,
                _pianoActions.Key8,
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
            _enabled = true;
        }
    
        public void DisableInput()
        {
            _pianoActions.Disable();
            if(!_enabled) return;
            _enabled = false;
        }

        private void OnKeyStarted(InputAction.CallbackContext context, int keyNumber)
        {
            _instrumentKeyPresser.PressKey(keyNumber);
        }
        
        private void OnKeyCanceled(InputAction.CallbackContext context, int keyNumber)
        {
            _instrumentKeyPresser.ReleaseKey(keyNumber);
        }
    }
}