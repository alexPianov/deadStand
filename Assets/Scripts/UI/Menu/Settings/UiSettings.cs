using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiSettings: MonoBehaviour
    {
        public Lean.Gui.LeanToggle vibrationToggle;
        public Lean.Gui.LeanToggle alarmToggle;
        public Lean.Gui.LeanToggle postProcessingToggle;
        public Lean.Gui.LeanToggle shadowsToggle;
        public Lean.Gui.LeanToggle bloodToggle;
        
        public UiSettingsSliderIcon musicSlider;
        public UiSettingsSliderIcon soundSlider;
        public Slider qualitySlider;
        public UiSettingsLanguage languageButton;
        
        public UiSettingsRegister settingsRegister;
        public UiSettingsRegion region;
        public TextMeshProUGUI appVersion;
    }
   
}