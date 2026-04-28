using System;

namespace Game.Runtime.StateMachineSystem
{
    public class BaseStateMachine<T> : IDisposable, IStateMachine<T> where T : IState
    {
        private T _currentState;
        private bool _hasState;
        
        public bool TryGetCurrentState(out T currentState)
        {
            currentState = _currentState;
            return _hasState;
        }
        
        public void ChangeState(T state)
        {
            if(_currentState != null)
                _currentState.Exit();
            state.Enter();
            _currentState = state;
            _hasState = true;
        }

        public void Dispose()
        {
            if(_currentState != null)
                _currentState.Exit();
            _hasState = false;
        }
    }
}