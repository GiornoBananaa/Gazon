using UnityEngine;

namespace Game.Runtime.CameraSystem
{
    public interface ICameraRotator
    {
        void InputLook(Vector2 lookInput);
    }
}