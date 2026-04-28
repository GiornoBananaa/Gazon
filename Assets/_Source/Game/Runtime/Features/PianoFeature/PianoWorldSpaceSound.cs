using Game.Runtime.CameraFeature;
using Game.Runtime.PianoRhythmSystem;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class PianoWorldSpaceSound : MonoBehaviour
    {
        [SerializeField] private Transform _audioPoint;
        [SerializeField] private float _minAudioDistance = 1f;
        [SerializeField] private float _maxAudioDistance = 20f;
        [SerializeField] private float _minVolume = 0.2f;
        
        private IPianoNoteTweener _noteTweener;
        private CameraHandle _cameraHandle;
        
        [Inject]
        public void Construct(IPianoNoteTweener noteTweener, CameraHandle cameraHandle)
        {
            _noteTweener = noteTweener;
            _cameraHandle = cameraHandle;
        }
        
        private void Update()
        {
            _noteTweener.SetPianoWorldPosition(_audioPoint.position);
            float distanceKoef = Mathf.InverseLerp(_maxAudioDistance, _minAudioDistance, Vector3.Distance(_audioPoint.position, _cameraHandle.transform.position));
            _noteTweener.SetSpatialBlend(distanceKoef);
            _noteTweener.SetMaxVolume(Mathf.Lerp(_minVolume, 1, distanceKoef));
        }

    }
}