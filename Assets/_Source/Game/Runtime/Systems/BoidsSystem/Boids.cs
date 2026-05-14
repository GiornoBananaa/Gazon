using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime.SpatialHashing;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

namespace Game.Runtime.BoidsSystem
{
    public class Boids : MonoBehaviour
    {
        [SerializeField] private BoidsEntity _prefab;
        [SerializeField] private Vector3 _areaSize;
        [SerializeField] private int _maxCount;
        [Header("Movement")]
        [SerializeField] private float _maxYVelocity;
        [SerializeField] private float _maxVelocity;
        [SerializeField] private float _rotationSpeed;
        [Header("Flocking")]
        [SerializeField] private float _nearEntitiesMaxRadius;
        [SerializeField] private float _nearEntitiesUpdateTime = 0.3f;
        [SerializeField] private float _spreadWeight = 0.05f; 
        [SerializeField] private float _velocityWeight = 1;
        [SerializeField] private float _positionWeight = 0.05f;
        [SerializeField] private float _separationWeight = 0.1f;
        [SerializeField] private float _maxSeparationDistance = 0.5f;
        [Header("Spatial Hashing")]
        [SerializeField] private float _spatialHashCellSize = 5f;
        
        private List<BoidsEntity> _boidsEntities;
        private SpatialHash<BoidsEntity> _spatialHash;
        
        private NativeArray<float3> _positions;
        private NativeArray<float3> _velocities;
        private NativeArray<float3> _accelerations;
        private NativeStream _nearObjectsStream;
        private TransformAccessArray _transformAccessArray;
        
        private CancellationTokenSource _cancellationTokenSource;
        
        public IReadOnlyList<BoidsEntity> Entities => _boidsEntities;
        
        public event Action OnBoidsGenerated;
        
        private void Start()
        {
            GenerateBoids();
            UpdateNearEntitiesJob(_spatialHash.UpdateJob()).Complete();
            _cancellationTokenSource = new CancellationTokenSource();
            _ = UpdateNearEntities(_cancellationTokenSource.Token);
        }

        public void Update()
        {
            if(_boidsEntities == null) return;
            
            var moveJob = new BoidsMoveJob()
            {
                Positions = _positions,
                Velocities = _velocities,
                Accelerations = _accelerations,
                DeltaTime = Time.deltaTime,
                MaxVelocity = _maxVelocity,
                RotationSpeed = _rotationSpeed
            };
            var accelerationJob = new BoidsAccelerationJob()
            {
                Positions = _positions,
                Velocities = _velocities,
                Accelerations = _accelerations,
                NearObjectReader = _nearObjectsStream.AsReader(),
                MaxSeparationDistance = _maxSeparationDistance,
                SpreadWeight = _spreadWeight,
                VelocityWeight = _velocityWeight,
                PositionWeight = _positionWeight,
                SeparationWeight = _separationWeight,
            };
            var boundsJob = new BoidsBoundsJob()
            {
                Positions = _positions,
                Accelerations = _accelerations,
                AreaPosition = transform.position,
                AreaSize = _areaSize
            };
            
            var moveHandle = moveJob.Schedule(_transformAccessArray);
            var accelerationHandle = accelerationJob.Schedule(_maxCount, 0, moveHandle);
            var boundsHandle = boundsJob.Schedule(_transformAccessArray, accelerationHandle);
            
            boundsHandle.Complete();
        }
        
        public void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            
            _positions.Dispose();
            _velocities.Dispose();
            _accelerations.Dispose();
            _transformAccessArray.Dispose();
            _spatialHash.Dispose();
            _nearObjectsStream.Dispose();
        }
        
        private void GenerateBoids()
        {
            _positions = new NativeArray<float3>(_maxCount, Allocator.Persistent);
            _velocities = new NativeArray<float3>(_maxCount, Allocator.Persistent);
            _accelerations = new NativeArray<float3>(_maxCount, Allocator.Persistent);
            
            _boidsEntities = new List<BoidsEntity>(_maxCount);
            
            Vector3 halfSize = _areaSize / 2f;
            
            var transforms = new Transform[_maxCount];
            for (var i = 0; i < _maxCount; i++)
            {
                var entity = Instantiate(_prefab, 
                    transform.position + new Vector3(Random.Range(-halfSize.x, halfSize.x), Random.Range(-halfSize.y, halfSize.y), Random.Range(-halfSize.z, halfSize.z)), 
                    Quaternion.identity);
                entity.gameObject.name = _prefab.name + " " + i;
                transforms[i] = entity.transform;
                _boidsEntities.Add(entity);
                
                Vector3 velocity = Random.insideUnitSphere;
                _velocities[i] = _maxVelocity * new Vector3(velocity.x, 0, velocity.z).normalized;
            }
            _transformAccessArray = new TransformAccessArray(transforms);
            
            if(_spatialHash != null) _spatialHash.Dispose();
            _spatialHash = new SpatialHash<BoidsEntity>(_boidsEntities, _spatialHashCellSize);
            
            OnBoidsGenerated?.Invoke();
        }
        
        private async UniTaskVoid UpdateNearEntities(CancellationToken token)
        {
            while (gameObject != null && _spatialHash != null && !token.IsCancellationRequested)
            {
                if(_nearEntitiesUpdateTime > 0)
                    await UniTask.WaitForSeconds(_nearEntitiesUpdateTime, cancellationToken: token);
                else
                    await UniTask.NextFrame(cancellationToken: token);
                UpdateNearEntitiesJob(_spatialHash.UpdateJob()).Complete();
            }
        }

        private JobHandle UpdateNearEntitiesJob(JobHandle dependsOn = default)
        {
            _nearObjectsStream.Dispose();
            _nearObjectsStream = new NativeStream(_maxCount, Allocator.Persistent);
            return _spatialHash.FindAllInRadiusParallel(_nearEntitiesMaxRadius, _nearObjectsStream, _boidsEntities.Count, 100, dependsOn);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, _areaSize);
        }
        
        [RequireComponent(typeof(Boids))]
        public abstract class BoidsComponent : MonoBehaviour
        {
            [SerializeField] protected Boids Boids;

            protected NativeArray<float3> Positions;
            protected NativeArray<float3> Velocities;
            protected NativeArray<float3> Accelerations;
            
            private void Awake()
            {
                Boids.OnBoidsGenerated += OnGenerated;
            }

            private void OnDestroy()
            {
                if(Positions.IsCreated)
                    Positions.Dispose();
                if(Velocities.IsCreated)
                    Velocities.Dispose();
                if(Accelerations.IsCreated)
                    Accelerations.Dispose();
                Boids.OnBoidsGenerated -= OnGenerated;
            }

            private void OnGenerated()
            {
                Positions = Boids._positions;
                Velocities = Boids._velocities;
                Accelerations = Boids._accelerations;
                OnBoidsGenerated();
            }
            
            protected virtual void OnBoidsGenerated()
            {
                Positions = Boids._positions;
                Velocities = Boids._velocities;
                Accelerations = Boids._accelerations;
            }
        }
    }
}