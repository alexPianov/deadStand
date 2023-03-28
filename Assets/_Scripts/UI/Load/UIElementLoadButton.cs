using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    [RequireComponent(typeof(Button))]
    public class UIElementLoadButton : UIElementLoadByType
    {
        [Inject] private CacheAudio _cacheAudio;
        public CacheAudio.MenuSound clickSound = CacheAudio.MenuSound.OnSwitchUi;
        
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(LoadScreen);
        }

        private void LoadScreen()
        {
            _cacheAudio.Play(clickSound, true);
            
            Load();
        }
    }
}
