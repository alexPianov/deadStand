using System;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiSoundOpenClose : MonoBehaviour
    {
        public CacheAudio.MenuSound OpenSound = CacheAudio.MenuSound.OnInventoryOpen;
        public CacheAudio.MenuSound CloseSound = CacheAudio.MenuSound.OnInventoryClose;
        
        [Inject] private CacheAudio _cacheAudio;

        private void Start()
        {
            _cacheAudio.Play(OpenSound);
        }

        private void OnDisable()
        {
            _cacheAudio.Play(CloseSound);
        }
    }
}