using EventBusSystem;
using UnityEngine;

namespace Playstel
{
    public class UiRotationSlider : MonoBehaviour, IRotationSliderMode
    {
        public Canvas sliderCanvas;
        
        private void OnEnable()
        {
            EventBus.Subscribe(this);
        }
        
        private void OnDisable()
        {
            EventBus.Unsubscribe(this);
        }
        
        public void HandlerActiveRotationSlider(bool state)
        {
            sliderCanvas.enabled = state;
        }
    }
}