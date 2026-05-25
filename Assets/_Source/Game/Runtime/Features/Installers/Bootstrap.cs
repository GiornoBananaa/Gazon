using Game.Runtime.MusicInstrumentSystem;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.Installers
{
    public class Bootstrap : MonoBehaviour
    {
        private MusicLibrary _musicLibrary;
        
        [Inject]
        private void Construct(MusicLibrary musicLibrary)
        {
            _musicLibrary = musicLibrary;
        }
        
        private void Awake()
        {
            _musicLibrary.Initialize();
        }
    }
}