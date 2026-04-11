using Game.Runtime.CameraSystem;
using Game.Runtime.Configs;
using UnityEngine;

namespace Game.Runtime.PlayerInteractionSystem
{
    public class PlayerRaycastInteraction : IPlayerInteraction
    {
        private readonly ICurrentCamera _currentCamera;
        private readonly float _interactionRange;
        private readonly LayerMask _interactableLayer;
        
        public PlayerRaycastInteraction(ICurrentCamera currentCamera, PlayerConfig playerConfig)
        {
            _currentCamera = currentCamera;
            _interactionRange = playerConfig.InteractionRange;
            _interactableLayer = playerConfig.InteractableLayer;
        }
        
        public void TryInteract()
        {
            var cameraTransform = _currentCamera.GetCurrentCamera().transform;
            
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, _interactionRange, _interactableLayer, QueryTriggerInteraction.Collide)) return;
            
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable == null) return;
            interactable.Interact();
        }
    }
}