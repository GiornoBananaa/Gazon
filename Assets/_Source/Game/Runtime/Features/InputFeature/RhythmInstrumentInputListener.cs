using Game.Runtime.PianoFeature;
using Game.Runtime.RhythmSystem;
using UnityEngine.InputSystem;

namespace Game.Runtime.InputFeature
{
    public class RhythmInstrumentInputListener : IInputListener
    {
        private readonly InputAction[] _noteActions;
        private GameInputActions.PianoRhythmActions _pianoActions;
        private readonly IRhythmSheet _rhythmSheet;
        private int _keysCount;
        
        private bool _enabled = true;
        
        public RhythmInstrumentInputListener(GameInputActions actionMap, IRhythmSheet rhythmSheet)
        {
            _pianoActions = actionMap.PianoRhythm;
            _rhythmSheet = rhythmSheet;
            
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

        public void SetKeysCount(int count)
        {
            _keysCount = count;
        }
        
        private void OnKeyStarted(InputAction.CallbackContext context, int keyNumber)
        {
            keyNumber -= (GlobalConstants.MAX_KEYS_COUNT - _keysCount) / 2;
            if(keyNumber < 0 || keyNumber >= _keysCount) return;
            _rhythmSheet.StartKey(keyNumber);
        }
        
        private void OnKeyCanceled(InputAction.CallbackContext context, int keyNumber)
        {
            keyNumber -= (GlobalConstants.MAX_KEYS_COUNT - _keysCount) / 2;
            if(keyNumber < 0 || keyNumber >= _keysCount) return;
            _rhythmSheet.StopKey(keyNumber);
        }
    }
}