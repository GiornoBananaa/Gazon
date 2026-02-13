namespace Game.Runtime.CameraSystem
{
    public class CurrentCamera : ICurrentCamera
    {
        private UnityEngine.Camera _currentCamera;
        
        public UnityEngine.Camera GetCurrentCamera()
        {
            return _currentCamera ??= UnityEngine.Camera.main;
        }
    }
}