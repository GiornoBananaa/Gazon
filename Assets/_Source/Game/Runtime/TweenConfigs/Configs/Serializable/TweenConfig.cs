using System;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.Serialization;

namespace DOTweenConfigs
{
    /// <summary>
    /// Base tween config for any other tween config. Contains
    /// duration parameter which any tweening in DOTween has.
    /// </summary>
    [Serializable]
    public class TweenConfig
    {
        public float Duration;
        public bool CustomEaseEnabled;
        public Ease Ease;
        public AnimationCurve CustomEase;
    }
}
