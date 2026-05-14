using DG.Tweening;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.TerrainChunkSystem
{
    public class AllBiomesLinker : MonoBehaviour
    {
        [Header("TeleportAnimation")]
        [SerializeField] private bool _animatedTeleport;
        [SerializeField] private float _hideHeight = -2f;
        [SerializeField] private float _animationTime = 1f;
        
        private PlanetMap _planetMap;
        private Tween _tween;
        private Vector2Int _chunk = new(-1, -1);
        private Vector2Int _localChunk = new(-1, -1);
        private Vector2Int _chunkBuf;
        private Vector3 _localPosition;
        private bool _isLinked;
        
        private float _defaultHeight;
        private bool _isAnimation;
        private bool _isAnimatedTeleported = true;
        
        [Inject]
        public void Construct(PlanetMap planetMap)
        {
            _planetMap = planetMap;
        }

        private void Awake()
        {
            _defaultHeight = transform.position.y;
        }

        private void Update()
        {
            if(_planetMap.Chunks.CurrentValue == null) return;
            UpdateCurrentChunk();
            UpdateCurrentBiome();
        }

        private void UpdateCurrentChunk()
        {
            if (_isLinked && _planetMap.IsOnChunk(transform.position, _chunk)) return;
            var chunk = _planetMap.FindChunk(transform.position);
            _chunk = chunk;
            _localChunk = GetLocalChunk(_chunk);
            transform.parent = _planetMap.Chunks.CurrentValue[chunk.x, chunk.y].transform;
            _localPosition = transform.localPosition;
            _isLinked = true;
        }
        
        private void UpdateCurrentBiome()
        {
            Vector2Int prevChunk = _chunk;
            Vector2Int prevChunkBuff = _chunkBuf;
            float nearestSqrDistance = float.MaxValue;
            Vector3 currentPos = _planetMap.GetChunkCenterPosition(_planetMap.CurrentChunkIndex.CurrentValue);
            for (int biomeX = 0; biomeX < _planetMap.Biomes.CurrentValue.GetLength(0); biomeX++)
            {
                for (int biomeY = 0; biomeY < _planetMap.Biomes.CurrentValue.GetLength(1); biomeY++)
                {
                    Vector2Int biome = new(biomeX, biomeY);
                    Vector2Int chunk = GetWorldChunk(_localChunk, biome);
                    Vector3 pos = _planetMap.Chunks.CurrentValue[chunk.x, chunk.y].transform.position + _localPosition;
                    float sqrDistance = (currentPos - pos).sqrMagnitude;
                    if(sqrDistance > nearestSqrDistance) continue;
                    nearestSqrDistance = sqrDistance;
                    _chunkBuf = chunk;
                }
            }
            
            if (_chunkBuf != prevChunk && _chunkBuf != prevChunkBuff)
            {
                if (!_animatedTeleport)
                {
                    _chunk = _chunkBuf;
                    transform.parent = _planetMap.Chunks.CurrentValue[_chunk.x, _chunk.y].transform;
                    transform.localPosition = _localPosition;
                    return;
                }
                _tween?.Kill();
                var sequence = DOTween.Sequence();
                {
                    _isAnimatedTeleported = false;
                    _isAnimation = true;
                    sequence.Join(transform.DOMoveY(_hideHeight, _animationTime).SetEase(Ease.InOutSine))
                        .AppendCallback(() =>
                        {
                            _isAnimatedTeleported = true;
                            _chunk = _chunkBuf;
                            transform.parent = _planetMap.Chunks.CurrentValue[_chunk.x, _chunk.y].transform;
                            transform.localPosition = _localPosition - new Vector3(0f, _defaultHeight - _hideHeight, 0f);
                            _isAnimation = false;
                        })
                        .Append(transform.DOMoveY(_defaultHeight, _animationTime));
                }
                _tween = sequence;
            }
            else if (_isAnimation && !_isAnimatedTeleported && _chunk == _chunkBuf)
            {
                _isAnimation = false;
                _isAnimatedTeleported = true;
                _tween?.Kill();
                transform.DOMoveY(_defaultHeight, _animationTime).SetEase(Ease.InOutSine);
            }
        }

        private Vector2Int GetLocalChunk(Vector2Int chunk) 
            => new(chunk.x % _planetMap.BiomeSizeInChunks.CurrentValue, chunk.y % _planetMap.BiomeSizeInChunks.CurrentValue);
        
        private Vector2Int GetWorldChunk(Vector2Int chunk, Vector2Int biome) 
            => new(chunk.x + biome.x * _planetMap.BiomeSizeInChunks.CurrentValue, chunk.y + biome.y * _planetMap.BiomeSizeInChunks.CurrentValue);
    }
}