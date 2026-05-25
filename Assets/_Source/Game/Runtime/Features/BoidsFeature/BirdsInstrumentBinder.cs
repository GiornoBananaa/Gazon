using System;
using Game.Runtime.BoidsSystem;
using Game.Runtime.Configs;
using Game.Runtime.PianoFeature;
using R3;
using UnityEngine;

namespace Game.Runtime.BoidsFeature
{
    public class BirdsInstrumentBinder : IInstrumentStatisticBinder, IDisposable
    {
        private readonly BoidsGravityPoints _boidsGravityPoints;
        private readonly BoidsPianoBindConfig _boidsPianoBindConfig;
        private readonly CompositeDisposable _disposable = new();

        private float _tone;
        
        public BirdsInstrumentBinder(InstrumentPlayStatistics statistics, BoidsGravityPoints boidsGravityPoints, BoidsPianoBindConfig boidsPianoBindConfig)
        {
            _boidsGravityPoints = boidsGravityPoints;
            _boidsPianoBindConfig = boidsPianoBindConfig;
            _disposable.Add(statistics.NotePressCountOverTime.Skip(1).Subscribe(OnNotePressCountOverTime));
            _disposable.Add(statistics.AverageToneOverTime.Skip(1).Subscribe(OnAverageToneOverTime));
        }

        private void OnNotePressCountOverTime(int notePressCountOverTime)
        {
            float forceCoef = Mathf.Clamp01((float)notePressCountOverTime / _boidsPianoBindConfig.NotesCountForMaxForce);
            float minForce = _boidsPianoBindConfig.MinForce;
            float maxForce = forceCoef * _boidsPianoBindConfig.MaxForce;
            int i = 0;
            
            foreach (var gravityPoint in _boidsGravityPoints.GravityPoints)
            {
                gravityPoint.MaxForce = Mathf.Lerp(maxForce, minForce, Mathf.Abs(_tone - ((float)i / _boidsGravityPoints.GravityPointsCount)));
                i++;
            }
        }
        
        private void OnAverageToneOverTime(float tone)
        {
            _tone = tone;
        }

        public void OnInstrumentStop()
        {
            
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}