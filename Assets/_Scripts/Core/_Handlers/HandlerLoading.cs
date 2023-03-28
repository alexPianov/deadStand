using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Playstel
{
    public class HandlerLoading : MonoBehaviour
    {
        [Header("Screen")]
        public GameObject loadingScreen;
        public TextMeshProUGUI loadingScreenText;
        
        [Header("Popup")]
        public UiTransparency blackScreen;
        
        [Header("Popup")]
        public GameObject loadingPopup;
        public TextMeshProUGUI loadingPopupText;

        [Header("Info")] 
        public GameObject infoPanel;
        public TextMeshProUGUI detailsText;
        public Slider installSlider;
        public UiTransparency sliderTransparency;
        
        public enum Text
        {
            Loading, Saving, Creating, Finding, Updating, Join, 
            Check, Subscribe, Unsubscribe, Confirm, Login, Sending, Restoring, Registering
        }
        
        public void OpenBlackScreen(bool state)
        {
            if (!blackScreen)
            {
                Debug.Log("Failed to find loading panel");
                return;
            }

            blackScreen.Transparency(!state);
        }

        public void OpenLoadingScreen(bool state, Text text = Text.Loading)
        {
            if (!loadingScreen)
            {
                Debug.Log("Failed to find loading panel");
                return;
            }

            loadingScreen.SetActive(state);
            
            if(loadingScreenText.text == text.ToString()) return;
            loadingScreenText.text = text.ToString();
        }

        public void OpenLoadingPopup(bool state, Text text = Text.Loading)
        {
            if (!loadingPopup)
            {
                Debug.Log("Failed to find loading panel");
                return;
            }
            
            loadingPopup.SetActive(state);
            
            loadingPopupText.text = text.ToString();
        }

        public void SetLoadingText(string text)
        {
            if(detailsText) detailsText.text = text;
        }

        public void ActiveLoadingText(bool state)
        {
            if(detailsText) detailsText.enabled = state;
        }

        public void ActiveLoadingSlider(bool state)
        {
            sliderTransparency.Transparency(!state);
        }

        public void ActiveInfoPanel(bool state)
        {
            infoPanel.SetActive(state); 
        }

        public void ActiveSeasonSlider(bool state, float minutes = 0)
        {
            _startSeasonDownload = state;
            
            if (state)
            {
                installSlider.value = 0;
                installSlider.maxValue = minutes * 60;
            }
            else
            {
                installSlider.value = 0;
            }
        }

        private bool _startSeasonDownload;
        private void Update()
        {
            if (_startSeasonDownload)
            {
                installSlider.value += Time.deltaTime;

                if (installSlider.value >= installSlider.maxValue)
                {
                    _startSeasonDownload = false;
                }
            }
        }
    }
}
