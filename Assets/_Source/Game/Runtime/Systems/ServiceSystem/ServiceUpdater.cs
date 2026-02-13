using System.Collections.Generic;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.ServiceSystem
{
    public class ServiceUpdater: MonoBehaviour
    {
        [Inject] private IEnumerable<IUpdatable> _updatables;

        private void Update()
        {
            foreach (var updatable in _updatables)
            {
                updatable.Update(Time.deltaTime);
            }
        }
    }
}