using System;
using DG.Tweening;
using Game.Runtime.Utils;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

namespace Game.Runtime.MusicInstrumentFeature.Animations
{
    public class HandRig : MonoBehaviour
    {
        [Serializable]
        public class FingerConfig
        {
            public TwoBoneIKConstraint IKConstraint;
            public float MaxDistanceToLastFinger = 0.05f;
            public float MaxFingerLength = 0.06f;
            [HideInInspector] public Vector3 DefaultTargetLocalPosition;
            [HideInInspector] public bool Released = true;
            [HideInInspector] public Tween Tween;
            [HideInInspector] public bool PositionIsLocal = true;
            [HideInInspector] public Vector3 TargetPosition;
            [HideInInspector] public Vector3 LastWorldTargetPosition;
        }
        
        [SerializeField] private FingerConfig[] _fingerTargets;
        [SerializeField] private Transform _rigRoot;
        [SerializeField] private Renderer _renderer;
        [SerializeField] private float _maxBodyAngle = 25f;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private bool _isRight;
        
        private bool _updateHand;
        private Vector3 _firstRootPosition;
        private Vector3 _lastRootPosition;
        private Vector3 _handTargetPosition;
        private Quaternion _handTargetRotation;
        private Tween _handTween;
        private Tween _transparencyTween;
        
        public bool IsRight => _isRight;
        public int FingersCount => _fingerTargets.Length;

        private void Awake()
        {
            foreach (var fingerTarget in _fingerTargets)
            {
                fingerTarget.DefaultTargetLocalPosition = _rigRoot.InverseTransformPoint(fingerTarget.IKConstraint.data.target.position);
                fingerTarget.TargetPosition = _rigRoot.InverseTransformPoint(fingerTarget.IKConstraint.data.target.position);
                fingerTarget.Released = true;
                fingerTarget.PositionIsLocal = true;
            }
            _handTargetPosition = _rigRoot.position;
            _handTargetRotation = _rigRoot.rotation;
        }

        private void Start()
        {
            _handTargetPosition = _rigRoot.position;
            _handTargetRotation = _rigRoot.rotation;
        }

        private void Update()
        {
            if(_updateHand)
            {
                UpdateHand();
                _updateHand = false;
            }
        }

        public float GetMaxLength(int finger)
        {
            return _fingerTargets[finger].MaxFingerLength;
        }
        
        public float GetMaxDistance(int finger, int otherFinger)
        {
            float distance = 0;
            for (int i = Mathf.Min(finger, otherFinger) + 1; i <= Mathf.Max(finger, otherFinger); i++)
            {
                distance += _fingerTargets[i].MaxDistanceToLastFinger;
            }
            return distance;
        }
        
        public void SetPosition(int finger, Vector3 position)
        {
            if(finger >= _fingerTargets.Length) return;
            _fingerTargets[finger].IKConstraint.weight = 1;
            _fingerTargets[finger].Released = false;
            _updateHand = true;
            SetFingerPosition(finger, position, false);
        }
        
        public void Release(int finger)
        {
            if(finger >= _fingerTargets.Length || _fingerTargets[finger].Released) return;
            _fingerTargets[finger].IKConstraint.weight = 1;
            _fingerTargets[finger].Released = true;
            _updateHand = true;
            SetFingerPosition(finger, CustomInverseTransformPoint(_handTargetPosition, _handTargetRotation, _rigRoot.lossyScale, _fingerTargets[finger].LastWorldTargetPosition) + new Vector3(0,0.01f,0), true);
        }
        
        public void ReleaseAll()
        {
            for (int i = 0; i < _fingerTargets.Length; i++)
            {
                Release(i);
            }
        }

        public void KillTween()
        {
            _handTween?.Kill();
        }

        public void SetRelaxedPosition(Vector3 position)
        {
            SetHandPosition(position, Quaternion.identity);
        }

        public void SetTransparency(float value, float duration)
        {
            _transparencyTween?.Kill();
            _transparencyTween = _renderer.material.DOFade(value, duration);
        }
        
        private void UpdateHand()
        {
            int first = -1;
            int last = -1;
            
            for (int i = 0; i < _fingerTargets.Length; i++)
            {
                if(_fingerTargets[i].Released) continue;
                if(first < 0) first = i;
                last = i;
            }
            
            for (int i = 0; i < _fingerTargets.Length; i++)
            {
                if(!_fingerTargets[i].Released)
                {
                    SetFingerPosition(i, _fingerTargets[i].TargetPosition, false);
                    continue;
                }

                Vector3 scaledPoint = new Vector3(_fingerTargets[i].TargetPosition.x * _rigRoot.lossyScale.x, _fingerTargets[i].TargetPosition.y * _rigRoot.lossyScale.y, _fingerTargets[i].TargetPosition.z * _rigRoot.lossyScale.z);
                float distanceToLastPosition = Vector3.Distance(_handTargetRotation * scaledPoint + _handTargetPosition, _fingerTargets[i].LastWorldTargetPosition);
                if(distanceToLastPosition > 0.01f)
                    SetFingerPosition(i, _fingerTargets[i].DefaultTargetLocalPosition, true);
            }
            
            if(first < 0) return;
            
            
            Vector2 a = _fingerTargets[first].TargetPosition.GetVectorXY();
            Vector2 b = _fingerTargets[last].TargetPosition.GetVectorXY();
            Vector2 rootC = _fingerTargets[last].IKConstraint.data.root.position.GetVectorXY();
            Vector2 rootD = _fingerTargets[first].IKConstraint.data.root.position.GetVectorXY();
            
            if (first == last)
            {
                Vector3 rotatedPosition = PredictChildPosition(_rigRoot, _fingerTargets[first].IKConstraint.data.root.position, Quaternion.identity);
                SetHandPosition(_rigRoot.position + (_fingerTargets[first].TargetPosition - rotatedPosition) + new Vector3(0, 0.05f, -_fingerTargets[first].MaxFingerLength), Quaternion.identity);
                return;
            }
            
            float ab = Vector2.Distance(a, b);
            float bc = _fingerTargets[last].MaxFingerLength;
            float cd = Vector2.Distance(rootC, rootD);
            float ad = _fingerTargets[first].MaxFingerLength;
            
            float baseDiff = Mathf.Abs(ab - cd);
            
            float fingerLength = ad > bc ? bc : ad;
            float h = Mathf.Sqrt(fingerLength * fingerLength - (baseDiff / 2) * (baseDiff / 2));
            
            Vector2 c = (a + b) / 2 + new Vector2(cd / 2, -h);
            Vector2 d = (a + b) / 2 + new Vector2(-cd / 2, -h);
            
            float height = _fingerTargets[first].TargetPosition.y + 0.05f;
            
            Vector3 localPoint1 = _rigRoot.InverseTransformPoint(_fingerTargets[first].IKConstraint.data.root.position);
            Vector3 localPoint2 = _rigRoot.InverseTransformPoint(_fingerTargets[last].IKConstraint.data.root.position);
            Vector3 targetPoint1 = d.GetVectorXZ(height);
            Vector3 targetPoint2 = c.GetVectorXZ(height);
            
            // Step 3: Calculate the vectors between the two points
            Vector3 localVector = localPoint1 - localPoint2;
            Vector3 targetVector =  targetPoint1 - targetPoint2;
            // Step 4: Calculate the rotation needed to bridge the gap
            Quaternion rotationDelta = Quaternion.FromToRotation(_rigRoot.TransformDirection(localVector), targetVector);

            // Step 5: Apply rotation while keeping the object pivoting around targetPoint1
            Quaternion rotation = MathUtils.ClampRotation(rotationDelta * _rigRoot.rotation, new Vector3(_maxBodyAngle, _maxBodyAngle, _maxBodyAngle));
            
            Vector3 targetPoint = ad > bc ? targetPoint2 : targetPoint1;
            
            // Step 1: Calculate the object's current global position for localPoint1
            Vector3 currentGlobalPoint = _fingerTargets[ad > bc ? last : first].IKConstraint.data.root.position;
            Vector3 futureGlobalPoint = PredictChildPosition(_rigRoot, currentGlobalPoint,rotation);
            SetHandPosition(_rigRoot.position - futureGlobalPoint + targetPoint, rotation);
        }
        
        private Vector3 PredictChildPosition(Transform parentTransform, Vector3 position, Quaternion futureParentRotation)
        {
            Vector3 relativePosition = position - parentTransform.position;
            Quaternion rotationDifference = futureParentRotation * Quaternion.Inverse(parentTransform.rotation);
            Vector3 predictedWorldPos = parentTransform.position + (rotationDifference * relativePosition);
            return predictedWorldPos;
        }
        
        private void SetHandPosition(Vector3 handPosition, Quaternion quaternion)
        {
            _handTween?.Kill();
            var sequence = DOTween.Sequence();
            float distance = Vector3.Distance(_rigRoot.position, handPosition);
            float duration = distance / Mathf.Lerp(_moveSpeed / 10, _moveSpeed, Mathf.Clamp01(distance / 0.15f));
            duration = Mathf.Max(0.18f, duration);
            if(Vector3.Distance(handPosition, _handTargetPosition) > 0.002f)
                sequence.Join(_rigRoot.DOJump(handPosition, 0.012f, 1, duration).SetEase(Ease.InOutSine));
            else
                sequence.Join(_rigRoot.DOMove(handPosition, duration).SetEase(Ease.InOutSine));
            sequence.Join(_rigRoot.DORotateQuaternion(quaternion, duration).SetEase(Ease.Linear));
            _handTargetPosition = handPosition;
            _handTargetRotation = quaternion;
            _handTween = sequence;
        }
        
        private void SetFingerPosition(int finger, Vector3 position, bool local)
        {
            if(!_fingerTargets[finger].PositionIsLocal)
                _fingerTargets[finger].LastWorldTargetPosition = _fingerTargets[finger].TargetPosition;
            bool lastIsLocal = _fingerTargets[finger].PositionIsLocal;
            _fingerTargets[finger].PositionIsLocal = local;
            if (local)
                _fingerTargets[finger].IKConstraint.data.target.parent = _rigRoot;
            else
                _fingerTargets[finger].IKConstraint.data.target.parent = null;
            Transform target = _fingerTargets[finger].IKConstraint.data.target;
            if(local == lastIsLocal && ((!local && Vector3.Distance(position, target.position) < 0.002f) 
                                        || (local && Vector3.Distance(position, target.localPosition) < 0.002f)))
            {
                _fingerTargets[finger].TargetPosition = position;
                return;
            }
            _fingerTargets[finger]?.Tween.Kill();
            
            var sequence = DOTween.Sequence();
            float distance = Vector3.Distance(target.position, local ? _rigRoot.TransformPoint(position) : position);
            float duration = distance / Mathf.Lerp(_moveSpeed / 10, _moveSpeed, Mathf.Clamp01(distance / 0.15f));
            duration = Mathf.Max(0.1f, duration);
            if(local)
                sequence.Join(target.DOLocalMove(position, duration).SetEase(Ease.InOutSine));
            else
                sequence.Join(target.DOJump(position, 0.012f, 1, duration).SetEase(Ease.Linear));
            
            _fingerTargets[finger].Tween = sequence;
            _fingerTargets[finger].TargetPosition = position;
        }
        
        private static Vector3 CustomInverseTransformPoint(Vector3 transformPosition, Quaternion transformRotation, Vector3 transformLossyScale, Vector3 worldPoint)
        {
            // 1. Create the Local-to-World matrix using Position, Rotation, and Scale
            Matrix4x4 localToWorld = Matrix4x4.TRS(transformPosition, transformRotation, transformLossyScale);
        
            // 2. Invert the matrix to create a World-to-Local matrix
            Matrix4x4 worldToLocal = localToWorld.inverse;
        
            // 3. Multiply the world point by the inverted matrix
            return worldToLocal.MultiplyPoint3x4(worldPoint);
        }
    }
}