using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiSettingsSliderIcon : MonoBehaviour
    {
        public Image valueImage;
        public Image valueImageOff;
        public Slider Slider;

        private float _sliderValue;
        private const int _minValue = 1;

        public void SetValue(Slider slider)
        {
            _sliderValue = slider.value;
            
            ChangeValueImage(_sliderValue > _minValue);
        }

        public void SetStartValue(float value)
        {
            _sliderValue = value;
            Slider.value = value;
            
            ChangeValueImage(_sliderValue > _minValue);
        }

        private void ChangeValueImage(bool isOn)
        {
            if (!valueImageOff) return;
            valueImage.enabled = isOn;
            valueImageOff.enabled = !isOn;
        }
    }
}