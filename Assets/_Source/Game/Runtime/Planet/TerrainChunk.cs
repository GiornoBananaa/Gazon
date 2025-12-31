using System;
using UnityEngine;

namespace Game.Runtime.Planet
{
    public class TerrainChunk : MonoBehaviour
    {
        [SerializeField] private Terrain _terrain;

        public void SetSize(float size)
        {
            /* not working...
            var terrainData = _terrain.terrainData;
            
            int newHeightmapResolution = Mathf.ClosestPowerOfTwo(
                Mathf.RoundToInt(terrainData.heightmapResolution * (size / terrainData.size.x))) + 1;
            Vector3 newSize = new Vector3(size, terrainData.size.y, size);
            
            terrainData.size = newSize;
            terrainData.heightmapResolution = newHeightmapResolution;*/
        }
    }
}