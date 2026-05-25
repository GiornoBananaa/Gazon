using Game.Runtime.CameraFeature;
using Game.Runtime.MusicInstrumentSystem;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class InstrumentWorldSpaceSound : MonoBehaviour
    {
        [SerializeField] private Transform _audioLineStart;
        [SerializeField] private Transform _audioLineEnd;
        [SerializeField] private float _minAudioDistance = 1f;
        [SerializeField] private float _maxAudioDistance = 20f;
        [SerializeField] private float _minVolume = 0.2f;
        
        private IInstrumentNoteTweener _noteTweener;
        private CameraHandle _cameraHandle;
        
        [Inject]
        public void Construct(IInstrumentNoteTweener noteTweener, CameraHandle cameraHandle)
        {
            _noteTweener = noteTweener;
            _cameraHandle = cameraHandle;
        }
        
        private void Update()
        {
            _noteTweener.SetPianoWorldPosition(_audioLineStart.position, _audioLineEnd.position);
            float distanceKoef = Mathf.InverseLerp(_maxAudioDistance, _minAudioDistance,
                Vector3.Distance((_audioLineStart.position + _audioLineEnd.position) / 2, _cameraHandle.transform.position));
            _noteTweener.SetSpatialBlend(distanceKoef);
            _noteTweener.SetMaxVolume(Mathf.Lerp(_minVolume, 1, distanceKoef));
        }

    }
}