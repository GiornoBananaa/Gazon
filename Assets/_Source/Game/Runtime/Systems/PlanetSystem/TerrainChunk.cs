using UnityEngine;

namespace Game.Runtime.PlanetSystem
{
    public class TerrainChunk : MonoBehaviour
    {
        [SerializeField] private Terrain _terrain;

        private Material _grassMaterial; 
        
        public Material GrassMaterial
        {
            get
            {
                if(_grassMaterial == null)
                    _grassMaterial = _terrain.terrainData.detailPrototypes[^1].prototype.GetComponent<Renderer>().sharedMaterial;
                
                return _grassMaterial;
            }
        }

        public Material TerrainMaterial => _terrain.materialTemplate;
    }
}