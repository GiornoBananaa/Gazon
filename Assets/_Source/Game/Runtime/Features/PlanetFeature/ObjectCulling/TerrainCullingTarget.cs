using Game.Runtime.ObjectCullingSystem;
using Game.Runtime.ObjectCullingSystem.CullingRules;
using R3;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.PlanetFeature.ObjectCulling
{
    public class TerrainCullingTarget : MonoBehaviour
    {
        [SerializeField] private Terrain _terrain; 
        
        [Inject] private ObjectCuller _culler;
        
        private ObjectCullingTarget _cullingTarget;

        private void Awake()
        {
            Vector3 size = new Vector3(_terrain.terrainData.bounds.size.x, 2.7f, _terrain.terrainData.bounds.size.z);
            _cullingTarget = new ObjectCullingTarget(transform, 
                new Bounds(_terrain.terrainData.bounds.center, size), 
                new IObjectCullingRule[]
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
            _terrain.enabled = isCulled;
        }
    }
}