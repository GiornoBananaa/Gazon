using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Runtime.BoidsSystem
{
    public class BoidsGravityPoints : Boids.BoidsComponent
    {
        [Serializable]
        public class GravityPoint
        {
            public Transform Transform;
            public float MinRadius;
            public float MaxRadius;
            public float MinForce;
            public float MaxForce;
        }
        
        [SerializeField] private List<GravityPoint> _gravityPoints;
        
        private NativeArray<float3> _gravityPositions;
        private NativeArray<float2> _gravityRadius;
        private NativeArray<float2> _gravityPointsForce;
        
        public IEnumerable<GravityPoint> GravityPoints => _gravityPoints;
        public int GravityPointsCount => _gravityPoints.Count;
        
        protected override void OnBoidsGenerated()
        {
            _gravityPositions = new NativeArray<float3>(_gravityPoints.Count, Allocator.Persistent);
            _gravityPointsForce = new NativeArray<float2>(_gravityPoints.Count, Allocator.Persistent);
            _gravityRadius = new NativeArray<float2>(_gravityPoints.Count, Allocator.Persistent);
        }
        
        private void Update()
        {
            for (int i = 0; i < _gravityPoints.Count; i++)
            {
                var point = _gravityPoints[i];
                _gravityPositions[i] = point.Transform.position;
                _gravityPointsForce[i] = new(point.MinForce, point.MaxForce);
                _gravityRadius[i] = new(point.MinRadius, point.MaxRadius);
            }
            
            var accelerationJob = new BoidsGravityPointJob()
            {
                Positions = Positions,
                Accelerations = Accelerations,
                GravityPoints = _gravityPositions,
                GravityPointsForce = _gravityPointsForce,
                GravityRadius = _gravityRadius,
            };
            
            accelerationJob.Schedule(Boids.Entities.Count, 0).Complete();
        }
        
        public void OnDestroy()
        {
            _gravityPositions.Dispose();
            _gravityPointsForce.Dispose();
            _gravityRadius.Dispose();
        }
        
        public void AddGravityPoint(GravityPoint gravityPoint)
        {
            if(_gravityPoints.Contains(gravityPoint)) return;
            
            _gravityPoints.Add(gravityPoint);
            
            if(_gravityPoints.Count <= _gravityPositions.Length) return;
            
            _gravityPositions.Dispose();
            _gravityPointsForce.Dispose();
            _gravityRadius.Dispose();
            OnBoidsGenerated();
        }
        
        public void RemoveGravityPoint(GravityPoint gravityPoint)
        {
            if(!_gravityPoints.Contains(gravityPoint)) return;
            
            _gravityPoints.Remove(gravityPoint);
            
            for (int i = _gravityPoints.Count; i < _gravityPointsForce.Length; i++)
            {
                _gravityPointsForce[i] = float2.zero;
            }
        }
        
        #region Gizmos
        
        private float _gizmoMaxForce = float.MinValue;
        private float _gizmoMinForce = float.MaxValue;

        private void OnValidate()
        { 
            _gizmoMaxForce = float.MinValue;
            _gizmoMinForce = float.MaxValue;
        }

        private void OnDrawGizmos()
        {
            Color addColor = new Color(0, -1f, 0, 0);
            int i = 0;
            foreach (var gravityPoint in _gravityPoints)
            {
                var c = Gizmos.color;
                Gizmos.DrawWireSphere(gravityPoint.Transform.position, gravityPoint.MinRadius);
                Gizmos.color = Color.yellow + Color.Lerp(Color.clear, addColor, Mathf.InverseLerp(_gizmoMinForce, _gizmoMaxForce, gravityPoint.MaxForce));
                Gizmos.DrawWireSphere(gravityPoint.Transform.position, gravityPoint.MaxRadius);
                Gizmos.color = c;
                i++;
                
                if(gravityPoint.MaxForce > _gizmoMaxForce) _gizmoMaxForce = gravityPoint.MaxForce;
                if(gravityPoint.MaxForce < _gizmoMinForce) _gizmoMinForce = gravityPoint.MaxForce;
            }
        }
        #endregion
    }
}