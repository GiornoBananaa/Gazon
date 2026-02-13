namespace Game.Runtime.ObjectCullingSystem
{
    public interface IObjectCullingRule
    {
        bool IsCullObject(UnityEngine.Camera cameraTransform, ObjectCullingTarget target);
    }
}