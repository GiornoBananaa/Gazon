using Game.Runtime.CameraSystem;
using Game.Runtime.Utils;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.ObjectCulling
{
    [RequireComponent(typeof(Renderer))]
    public class RoundWorldCullingBounds : MonoBehaviour
    {
        [Inject] private ICurrentCamera _currentCamera; 
        [SerializeField] private Renderer _renderer;
        [SerializeField] private MeshFilter _filter;

        private Vector3 _localCenter;

        private void Awake()
        {
            _localCenter = _filter.mesh.bounds.center;
        }

        private void Update()
        {
            _renderer.bounds = new Bounds(MathUtils.ConvertToRoundWorldPosition(_renderer.transform.position + _localCenter, 
                _currentCamera.GetCurrentCamera().transform.position), _renderer.bounds.size);
        }
    }
}