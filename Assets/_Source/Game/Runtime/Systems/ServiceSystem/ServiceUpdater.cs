using System.Collections.Generic;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.ServiceSystem
{
    public class ServiceUpdater: MonoBehaviour
    {
        [Inject] private IEnumerable<IUpdatable> _updatables;
        [Inject] private IEnumerable<IFixedUpdatable> _fixedUpdatables;
        [Inject] private IEnumerable<ILateUpdatable> _lateUpdatables;

        private void Update()
        {
            foreach (var updatable in _updatables)
            {
                updatable.Update();
            }
        }
        
        private void FixedUpdate()
        {
            foreach (var updatable in _fixedUpdatables)
            {
                updatable.FixedUpdate();
            }
        }
        
        private void LateUpdate()
        {
            foreach (var updatable in _lateUpdatables)
            {
                updatable.LateUpdate();
            }
        }
    }
}