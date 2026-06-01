using UnityEngine;

namespace Game.Runtime.PlayerInteractionSystem
{
    public interface IInteractable
    {
        void Interact();
    }
    
    public interface IContinuousInteractable
    {
        void OnStartInteraction();
        void OnEndInteraction();
    }
    
    public interface ILookChangedListener
    {
        void OnLookChanged(Vector2 look);
    }
}