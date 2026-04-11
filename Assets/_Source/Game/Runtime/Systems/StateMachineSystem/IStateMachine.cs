namespace Game.Runtime.StateMachineSystem
{
    public interface IStateMachine<T> where T : IState
    {
        bool TryGetCurrentState(out T currentState);
        void ChangeState(T state);
    }
}