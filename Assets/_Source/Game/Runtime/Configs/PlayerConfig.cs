using UnityEngine;

namespace Game.Runtime.Configs
{
    [CreateAssetMenu(fileName = "Player", menuName = "Game/Player")]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Movement")]
        public float Speed = 6.5f;
        public float SprintSpeed = 10f;
        
        [Header("Interaction")]
        public float InteractionRange = 2f;
        public LayerMask InteractableLayer;
    }
}