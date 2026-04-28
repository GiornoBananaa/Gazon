using Game.Runtime.CameraSystem;
using Game.Runtime.StateMachineSystem;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.PlayerFeature
{
    public class PlayerSetup : MonoBehaviour
    {
        [SerializeField] private Transform _cameraPoint;

        [Inject]
        private void Construct(CameraFollowTargetMover cameraFollow, IStateMachine<IPlayerState> playerStateMachine, PlayerFreeWalkState playerFreeWalkState)
        {
            cameraFollow.SetTarget(_cameraPoint);
            cameraFollow.Enable();
            playerStateMachine.ChangeState(playerFreeWalkState);
        }
    }
}