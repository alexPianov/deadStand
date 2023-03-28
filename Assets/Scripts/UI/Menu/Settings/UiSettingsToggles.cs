using System;
using Lean.Gui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiSettingsToggles: MonoBehaviour
    {
        [Inject] private CacheUserSettings _cacheUserSettings;
        
        public void EditPushAlarmSetting(LeanToggle toggle)
        {
            var result = 1;
            if (toggle.On) result = 0;
            _cacheUserSettings.SetPushAlarms(result);
        }
        
        public void EditVibrationSetting(LeanToggle toggle)
        {
            var result = 1;
            if (toggle.On) result = 0;
            _cacheUserSettings.SetVibrations(result);
        }
        
        public void EditPostProcessingSetting(LeanToggle toggle)
        {
            var result = 1;
            if (toggle.On) result = 0;
            _cacheUserSettings.SetPostProcessing(result);
        }
        
        public void EditShadowsSetting(LeanToggle toggle)
        {
            var result = 1;
            if (toggle.On) result = 0;
            _cacheUserSettings.SetShadows(result);
        }

        public void EditBloodSetting(LeanToggle toggle)
        {
            var result = 1;
            if (toggle.On) result = 0;
            _cacheUserSettings.SetBlood(result);
        }
    }
   
}