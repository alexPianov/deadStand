using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiSettingsLoad : MonoBehaviour
    {
        public UiTransparency leftSidePanel;
        
        [Inject] private CacheUserSettings _cacheUserSettings;
        
        private UiSettings _ui;
        private void Awake()
        {
            _ui = GetComponent<UiSettings>();
        }
        
        [Inject] private CacheUserInfo _cacheUserInfo;
        [Inject] private CacheTitleData _cacheTitleData;

        public void Start()
        {
            _ui.languageButton.SetLanguageImage(_cacheUserSettings.pickedLanguage);
            _ui.musicSlider.SetStartValue(_cacheUserSettings.musicLevel);
            _ui.soundSlider.SetStartValue(_cacheUserSettings.soundLevel);
            _ui.qualitySlider.value = _cacheUserSettings.qualityLevel;
            _ui.alarmToggle.On = _cacheUserSettings.PushAlarms;
            _ui.vibrationToggle.On = _cacheUserSettings.Vibration;
            _ui.postProcessingToggle.On = _cacheUserSettings.PostProcessing;
            _ui.shadowsToggle.On = _cacheUserSettings.Shadows;
            _ui.bloodToggle.On = _cacheUserSettings.Blood;
            _ui.region.SetCurrentRegion(_cacheUserSettings.pickedRegionNumber);
            
            SetEmail();
            SetAppVersion();
            
            leftSidePanel.Transparency(false);
        }
        
        public void SetEmail()
        {
            var contactEmail = _cacheUserInfo.payload
                .GetPlayFabPayload().AccountInfo.PrivateInfo.Email;
            
            if (contactEmail == null)
            {
                _ui.settingsRegister.SetEmail(null);
            }
            else
            {
                _ui.settingsRegister.SetEmail(contactEmail);
            }
        }
        
        private void SetAppVersion()
        {
            _ui.appVersion.text = _cacheTitleData.GetTitleData(CacheTitleData.TitleDataKey.AppVersion);
        }
    }
   
}