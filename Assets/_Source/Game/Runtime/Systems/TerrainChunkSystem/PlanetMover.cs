using Game.Runtime.CameraSystem;
using Game.Runtime.ServiceSystem;
using UnityEngine;

namespace Game.Runtime.TerrainChunkSystem
{
    public class PlanetMover : IUpdatable
    {
        private readonly ICurrentCamera _currentCamera;
        private readonly PlanetMap _planetMap;

        public PlanetMover(PlanetMap planetMap, ICurrentCamera currentCamera)
        {
            _planetMap = planetMap;
            _currentCamera = currentCamera;
        }
        
        public void Update()
        {
            TerrainChunk[,] chunks = _planetMap.Chunks.CurrentValue;
            float chunkSize = _planetMap.ChunkSize.CurrentValue;
            
            float maxDistanceX = chunks.GetLength(0) * chunkSize / 2;
            float maxDistanceZ = chunks.GetLength(1) * chunkSize / 2;

            Vector3 cameraPosition = _currentCamera.GetCurrentCamera().transform.position;
            cameraPosition.y = 0;

            int chunksRows = chunks.GetLength(0);
            int chunksColumn = chunks.GetLength(1);
            
            for (int i = 0; i < chunksRows; i++)
            {
                for (int j = 0; j < chunksColumn; j++)
                {
                    TerrainChunk terrainChunk = chunks[i, j];
                    Vector3 chunkPosition = terrainChunk.transform.position;
                    float distanceX = chunkPosition.x + chunkSize / 2 - cameraPosition.x;
                    float distanceZ = chunkPosition.z + chunkSize / 2  - cameraPosition.z;
                    
                    if (distanceX > maxDistanceX)
                        terrainChunk.transform.position = new Vector3(chunkPosition.x - chunksRows * chunkSize, chunkPosition.y, chunkPosition.z);
                    if (distanceX < -maxDistanceX)
                        terrainChunk.transform.position = new Vector3(chunkPosition.x + chunksRows * chunkSize, chunkPosition.y, chunkPosition.z);
                    if (distanceZ > maxDistanceZ)
                        terrainChunk.transform.position = new Vector3(chunkPosition.x, chunkPosition.y, chunkPosition.z - chunksColumn * chunkSize);
                    if (distanceZ < -maxDistanceZ)
                        terrainChunk.transform.position = new Vector3(chunkPosition.x, chunkPosition.y, chunkPosition.z + chunksColumn * chunkSize);
                }
            }
        }
    }
}