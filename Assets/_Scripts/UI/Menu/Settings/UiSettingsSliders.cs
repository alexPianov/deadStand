using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiSettingsSliders: MonoBehaviour
    {
        [Inject] private CacheUserSettings _cacheUserSettings;
        [Inject] private CacheAudio _cacheAudio;
        
        public void EditMusicValue(Slider slider)
        {
            _cacheUserSettings.SetMusicLevel(slider.value);
        }

        public void EditSoundValue(Slider slider)
        {
            _cacheAudio.Play(CacheAudio.MenuSound.OnProcess);
            _cacheUserSettings.SetSoundLevel(slider.value);
        }

        public void EditQualityValue(Slider slider)
        {
            _cacheUserSettings.SetQualityLevel(slider.value);
        }
    }
   
}