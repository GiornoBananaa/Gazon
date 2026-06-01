using UnityEngine;

namespace Game.Runtime.PlayerInteractionSystem
{
    public interface IPlayerInteraction
    {
        void TryInteract();
        void TryStartContinuousInteraction();
        void EndContinuousInteraction();
        void OnLookChanged(Vector2 look);
    }
}