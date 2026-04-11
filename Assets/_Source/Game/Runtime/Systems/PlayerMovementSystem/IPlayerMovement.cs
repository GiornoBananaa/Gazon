using UnityEngine;

namespace Game.Runtime.PlayerMovementSystem
{
    public interface IPlayerMovement
    {
        void Enable();
        void Disable();
        void SetMoveDirection(Vector3 moveDirection);
    }
}