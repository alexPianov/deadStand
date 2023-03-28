using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiSettingsLanguage : MonoBehaviour
    {
        public TextMeshProUGUI languageName;
        public Image languageImage;
        public Button button;
        public GameObject languagePanel;
        public Transform panelFocus;
        private string currentLanguage;

        [Inject] private CacheSprites _cacheSprites;
        
        [Inject] private CacheUserSettings _cacheUserSettings;
        
        public enum Language
        {
            English, German, Russia
        }

        public void Awake()
        {
            button.onClick.AddListener(Active);
        }

        public string GetLanguage()
        {
            return currentLanguage;
        }

        public void SetLanguageImage(string language)
        {
            currentLanguage = language;
            languageName.text = language;
            
            var result = _cacheSprites.GetSpriteFromAtlas(language, ItemInfo.Catalog.Setup);
            
            if (result == null)
            {
                languageImage.enabled = false;
            }
            else
            {
                languageImage.sprite = result;
                languageImage.enabled = true;
            }
        }

        public void SaveLanguageSettings(string language)
        {
            _cacheUserSettings.SetLanguage(language);
        }

        private void Active()
        {
            OpenLanguagePanel(true);
        }

        public void OpenLanguagePanel(bool state)
        {
            languagePanel.SetActive(state);
        }
    }
}