using Game.Runtime.CameraSystem;
using Game.Runtime.Configs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Runtime.PlayerInteractionSystem
{
    public class PlayerRaycastInteraction : IPlayerInteraction
    {
        private readonly ICurrentCamera _currentCamera;
        private readonly float _interactionRange;
        private readonly LayerMask _interactableLayer;
        private readonly float _radius;
        
        private readonly CameraInputRotator _cameraInputRotator;
        private IContinuousInteractable _currentInteractable;
        private ILookChangedListener _lookListener;
        
        public PlayerRaycastInteraction(ICurrentCamera currentCamera, PlayerConfig playerConfig, CameraInputRotator cameraInputRotator)
        {
            _currentCamera = currentCamera;
            _interactionRange = playerConfig.InteractionRange;
            _interactableLayer = playerConfig.InteractableLayer;
            _radius = playerConfig.SphereCastRadius;
            _cameraInputRotator = cameraInputRotator;
        }
        
        public void TryInteract()
        {
            Ray ray = _currentCamera.GetCurrentCamera().ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.SphereCast(ray, _radius, out RaycastHit hit, _interactionRange, _interactableLayer, QueryTriggerInteraction.Collide)) return;
            
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable == null) return;
            interactable.Interact();
        }
        
        public void TryStartContinuousInteraction()
        {
            Ray ray = _currentCamera.GetCurrentCamera().ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.SphereCast(ray, _radius, out RaycastHit hit, _interactionRange, _interactableLayer, QueryTriggerInteraction.Collide)) return;
            
            _currentInteractable = hit.collider.GetComponent<IContinuousInteractable>();
            if (_currentInteractable == null) return;
            _currentInteractable.OnStartInteraction();
            if (_currentInteractable is ILookChangedListener listener)
                _lookListener = listener;
            
            _cameraInputRotator.Disable(GetHashCode());
        }
        
        public void EndContinuousInteraction()
        {
            if(_currentInteractable == null) return;
            _currentInteractable.OnEndInteraction();
            _currentInteractable = null;
            _lookListener = null;
            _cameraInputRotator.Enable(GetHashCode());
        }
        
        public void OnLookChanged(Vector2 look)
        {
            _lookListener?.OnLookChanged(look);
        }
    }
}