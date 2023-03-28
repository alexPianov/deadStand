using EventBusSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Playstel
{
    [RequireComponent(typeof(Slider))]
    public class GuiAnimWait : MonoBehaviour, IWaitHandler
    {
        private Slider _slider;

        private void OnEnable()
        {
            EventBus.Subscribe(this);
        }
        
        private void OnDisable()
        {
            EventBus.Unsubscribe(this);
        }

        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        public void HandleMaxWaitTime(float maxWaitTime)
        {
            _slider.maxValue = maxWaitTime;
            _slider.image.CrossFadeAlpha(1, 0, true);
            _slider.image.CrossFadeAlpha(0.1f, maxWaitTime, true);
        }

        public void HandleWaitTime(float waitTime)
        {
            _slider.value = waitTime;
        }
    }
}
