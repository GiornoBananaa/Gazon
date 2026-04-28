using Reflex.Core;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;

namespace Game.Runtime.Installers
{
    public class SelfSubContainerInjector : MonoBehaviour
    {
        public void Awake()
        {
            var parentContainer = gameObject.scene.GetSceneContainer();
            using var container = parentContainer.Scope(BuilderExtend);
            GameObjectInjector.InjectRecursive(gameObject, container);
        }

        protected virtual void BuilderExtend(ContainerBuilder builder) { }
    }
}