using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel.UI
{
    public class GuiButtonAudio : MonoBehaviour
    {
        public Image audioImage;
        public Sprite noAudioSprite;
        public Sprite audioSprite;

        [Inject] private CacheUserSettings _cacheUserSettings;

        public void Start()
        {
            audioSprite = audioImage.sprite;
            GetComponent<Button>().onClick.AddListener(Button);
        }

        public void Button()
        {
            Debug.Log("Button");
            
            if (!_cacheUserSettings.disableAudio)
            {
                _cacheUserSettings.DisableAudioMaster(true);
                audioImage.sprite = noAudioSprite;
            }
            else
            {
                _cacheUserSettings.DisableAudioMaster(false);
                audioImage.sprite = audioSprite;
            }
        }
    }
}
