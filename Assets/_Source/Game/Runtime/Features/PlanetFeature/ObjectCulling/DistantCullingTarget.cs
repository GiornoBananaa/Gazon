using Game.Runtime.ObjectCullingSystem;
using Game.Runtime.ObjectCullingSystem.CullingRules;
using R3;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.PlanetFeature.ObjectCulling
{
    public class RoundWorldCullingTarget : MonoBehaviour
    {
        [SerializeField] private float _distance; 
        [SerializeField] private Renderer _renderer; 
        
        [Inject] private ObjectCuller _culler;
        
        private ObjectCullingTarget _cullingTarget;

        private void Awake()
        {
            _cullingTarget = new ObjectCullingTarget(transform, new Bounds(_renderer.bounds.center - transform.position, _renderer.bounds.size), new IObjectCullingRule[]
            {
                new RoundWorldViewCullingRule()
            });
            
            _cullingTarget.IsCulled.Subscribe(OnCulledChanged).AddTo(this);
            _culler.Subscribe(_cullingTarget);
        }

        private void OnDestroy()
        { 
            _culler.Unsubscribe(_cullingTarget);
        }

        private void OnCulledChanged(bool isCulled)
        {
            transform.gameObject.SetActive(isCulled);
        }
    }
}