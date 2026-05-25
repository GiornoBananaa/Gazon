using Reflex.Core;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;

namespace Game.Runtime.Installers
{
    public class SelfSubContainerInjector : MonoBehaviour
    {
        private Container _container;
        
        public void Awake()
        {
            var parentContainer = gameObject.scene.GetSceneContainer();
            _container = parentContainer.Scope(BuilderExtend);
            GameObjectInjector.InjectRecursive(gameObject, _container);
        }

        protected virtual void BuilderExtend(ContainerBuilder builder) { }
    }
}