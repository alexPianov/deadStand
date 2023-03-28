using UnityEngine;
using UnityEngine.UI;

namespace Playstel
{
    public class UiSettingsLanguageButton : MonoBehaviour
    {
        public UiSettingsLanguage.Language currentLanguage;
        public UiSettingsLanguage UiSettingsLanguage;

        public void Awake()
        {
            GetComponent<Button>().onClick.AddListener(Active);
        }

        private void Active()
        {
            UiSettingsLanguage.SaveLanguageSettings(currentLanguage.ToString());
            UiSettingsLanguage.SetLanguageImage(currentLanguage.ToString());
            SetFocus();
        }

        private void SetFocus()
        {
            UiSettingsLanguage.panelFocus.SetParent(transform);
            UiSettingsLanguage.panelFocus.localPosition = new Vector3(0, 0, 0);
        }
    }
}