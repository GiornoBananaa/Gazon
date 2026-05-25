using System.Collections.Generic;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.ServiceSystem
{
    public class ServiceUpdater: MonoBehaviour
    {
        private List<IUpdatable> _updatables;
        private List<IFixedUpdatable> _fixedUpdatables;
        private List<ILateUpdatable> _lateUpdatables;

        [Inject]
        private void Construct(IEnumerable<IUpdatable> updatables, 
            IEnumerable<IFixedUpdatable> fixedUpdatables, IEnumerable<ILateUpdatable> lateUpdatables)
        {
            _updatables = new List<IUpdatable>(updatables);
            _fixedUpdatables = new List<IFixedUpdatable>(fixedUpdatables);
            _lateUpdatables = new List<ILateUpdatable>(lateUpdatables);
        }
        
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

        public void Add(IUpdatable updatable)
        {
            _updatables.Add(updatable);
        }
        
        public void Add(IFixedUpdatable updatable)
        {
            _fixedUpdatables.Add(updatable);
        }
        
        public void Add(ILateUpdatable updatable)
        {
            _lateUpdatables.Add(updatable);
        }
    }
}