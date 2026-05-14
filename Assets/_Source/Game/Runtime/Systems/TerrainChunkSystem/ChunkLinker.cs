using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.TerrainChunkSystem
{
    public class ChunkLinker : MonoBehaviour
    {
        private PlanetMap _planetMap;
        private Vector2Int _chunk = new(-1, -1);
        private bool _isLinked;
        
        [Inject]
        public void Construct(PlanetMap planetMap)
        {
            _planetMap = planetMap;
        }

        private void Update()
        {
            if(_planetMap.Chunks.CurrentValue == null) return;
            UpdateCurrentChunk();
        }
        
        private void UpdateCurrentChunk()
        {
            if (_isLinked && _planetMap.IsOnChunk(transform.position, _chunk)) return;
            var chunk = _planetMap.FindChunk(transform.position);
            _chunk = chunk;
            transform.parent = _planetMap.Chunks.CurrentValue[chunk.x, chunk.y].transform;
            _isLinked = true;
        }
    }
}