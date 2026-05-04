using UnityEngine;

namespace Game.Runtime.BoidsSystem
{
    public class BoidsEntity : MonoBehaviour
    {
        [field: SerializeField] public Renderer Renderer;
        [field: SerializeField] public MeshFilter Filter;
    }
}