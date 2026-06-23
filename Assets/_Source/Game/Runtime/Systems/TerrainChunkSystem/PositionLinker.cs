using DG.Tweening;
using Game.Runtime.CameraSystem;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.TerrainChunkSystem
{
    public class PositionLinker : MonoBehaviour
    {
        private PlanetMap _planetMap;
        private ICurrentCamera _currentCamera;
        private Tween _tween;
        private Vector2Int _chunk = new(-1, -1);
        private Vector3 _localPosition;
        private Vector3 _lastPosition;
        private bool _isLinked;
        
        private bool _isAnimation;
        
        [Inject]
        public void Construct(ICurrentCamera currentCamera, PlanetMap planetMap)
        {
            _planetMap = planetMap;
            _currentCamera = currentCamera;
        }

        private void Update()
        {
            if(_planetMap.Chunks.CurrentValue == null) return;
            UpdateCurrentChunk();
        }

        private void UpdateCurrentChunk()
        {
            if (!_isLinked)
            {
                _chunk = _planetMap.FindChunk(transform.position);
                _localPosition = transform.position - _planetMap.Chunks.CurrentValue[_chunk.x, _chunk.y].transform.position;
                _isLinked = true;
            }
            
            Vector3 cameraPos = _currentCamera.GetCurrentCamera().transform.position;
            
            TerrainChunk[,] chunks = _planetMap.Chunks.CurrentValue;
            float chunkSize = _planetMap.ChunkSize.CurrentValue;
            
            Vector3 terrainChunk = _planetMap.Chunks.CurrentValue[_chunk.x, _chunk.y].transform.position;
            
            int chunksRows = chunks.GetLength(0);
            int chunksColumn = chunks.GetLength(1);
            
            float sqrDistanceToLastPos = (cameraPos - transform.position).sqrMagnitude;
            float newSqrDistance = (cameraPos - transform.position).sqrMagnitude;

            Vector3 a = new Vector3(terrainChunk.x - chunksRows * chunkSize, terrainChunk.y, terrainChunk.z) + _localPosition;
            Vector3 b = new Vector3(terrainChunk.x + chunksRows * chunkSize, terrainChunk.y, terrainChunk.z) + _localPosition;
            Vector3 c = new Vector3(terrainChunk.x, terrainChunk.y, terrainChunk.z - chunksColumn * chunkSize) + _localPosition;
            Vector3 d = new Vector3(terrainChunk.x, terrainChunk.y, terrainChunk.z + chunksColumn * chunkSize) + _localPosition;
            Vector3 e = new Vector3(terrainChunk.x, terrainChunk.y, terrainChunk.z) + _localPosition;
            Vector3 pos = transform.position;

            float sqrDistance = (cameraPos - a).sqrMagnitude;
            if (sqrDistance < newSqrDistance)
            {
                pos = a;
                newSqrDistance = sqrDistance;
            }
            sqrDistance = (cameraPos - b).sqrMagnitude;
            if (sqrDistance < newSqrDistance)
            {
                pos = b;
                newSqrDistance = sqrDistance;
            }
            sqrDistance = (cameraPos - c).sqrMagnitude;
            if (sqrDistance < newSqrDistance)
            {
                pos = c;
                newSqrDistance = sqrDistance;
            }
            sqrDistance = (cameraPos - d).sqrMagnitude;
            if (sqrDistance < newSqrDistance)
            {
                pos = d;
                newSqrDistance = sqrDistance;
            }
            sqrDistance = (cameraPos - e).sqrMagnitude;
            if (sqrDistance < newSqrDistance)
            {
                pos = e;
                newSqrDistance = sqrDistance;
            }
            
            if (sqrDistanceToLastPos < newSqrDistance) return;
            transform.position = pos;
        }
    }
}