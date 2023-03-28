using Assets.SimpleLocalization;
using TMPro;
using UniRx;
using UnityEngine;

namespace Playstel
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizationAgent : MonoBehaviour, ITextPreprocessor
    {
        private TextMeshProUGUI currentText;
        private string defaultText;

        public string PreprocessText(string text)
        {
            Debug.Log("Preprocess text " + text);
            return text;
        }

        public void Awake()
        {
            currentText = GetComponent<TextMeshProUGUI>();
            defaultText = currentText.text;

            StartLocalize();
            
            LocalizationManager.LocalizationChanged += StartLocalize;
        }

        public void OnDestroy()
        {
            LocalizationManager.LocalizationChanged -= StartLocalize;
        }

        private void StartLocalize()
        {
            if (!currentText) return;
            
            if (string.IsNullOrEmpty(defaultText)) return;
            if (string.IsNullOrWhiteSpace(defaultText)) return;

            SetLocalizeFlag(defaultText);
        }

        public void SetLocalizeFlag(string localizeFlag = null)
        {
            if (string.IsNullOrEmpty(localizeFlag))
            {
                localizeFlag = defaultText;
            }
            
            var localizationText = LocalizationManager.Localize(localizeFlag);
            
            if (string.IsNullOrEmpty(localizationText)) return;
            
            currentText.text = FirstUpper(localizationText);
        }

        public static string FirstUpper(string str)
        {
            return str.Substring(0, 1).ToUpper() + (str.Length > 1 ? str.Substring(1) : "");
        }

        string language;
        private void SetFont()
        {
            // if (!localizator) return;
            //
            // if (LocalizationManager.Language == null) return;
            //
            // language = LocalizationManager.Language;
            //
            // if (localizator.chineseFonts.Contains(language))
            // {
            //     currentText.font = localizator.chineseFontAsset;
            //     return;
            // }
            //
            // if (localizator.japaneseFonts.Contains(language))
            // {
            //     currentText.font = localizator.japaneseFontAsset;
            //     return;
            // }
            //
            // if (localizator.europeFonts.Contains(language))
            // {
            //     currentText.font = localizator.standartFontAsset;
            //     return;
            // }
            //
            // if (localizator.cyrillicFonts.Contains(language))
            // {
            //     currentText.font = localizator.cyrillicFontAsset;
            //     return;
            // }
            //
            // if (localizator.koreanFonts.Contains(language))
            // {
            //     currentText.font = localizator.koreanFontAsset;
            //     return;
            // }
            //
            // if (localizator.greekFonts.Contains(language))
            // {
            //     currentText.font = localizator.greekFontAsset;
            //     return;
            // }
            //
            // if (localizator.arabicFonts.Contains(language))
            // {
            //     currentText.font = localizator.arabicFontAsset;
            //     return;
            // }
            //
            // if (localizator.thaiFonts.Contains(language))
            // {
            //     currentText.font = localizator.thaiFontAsset;
            //     return;
            // }
            //
            // if (localizator.vietFonts.Contains(language))
            // {
            //     currentText.font = localizator.vietFontAsset;
            //     return;
            // }
            //
            // if (localizator.hindiFonts.Contains(language))
            // {
            //     currentText.font = localizator.hindiFontAsset;
            //     return;
            //}
        }
    }
}
