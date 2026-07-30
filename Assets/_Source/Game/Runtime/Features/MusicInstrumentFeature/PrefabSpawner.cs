using UnityEngine;

namespace Game.Runtime.MusicInstrumentFeature
{
    public class PrefabSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;

        public void Awake()
        {
            Instantiate(_prefab, transform.position, transform.rotation);
        }
    }
}