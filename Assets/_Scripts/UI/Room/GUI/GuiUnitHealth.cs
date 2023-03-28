using System;
using EventBusSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel.UI
{
    public class GuiUnitHealth : MonoBehaviour
    {
        public TextMeshProUGUI unitHealth;
        public Image healthColor;
        public Slider healthSlider;
        public Image healthBackground;

        [Inject]
        private CacheSprites _cacheSprites;

        private Canvas _healthCanvas;

        private void Awake()
        {
            _healthCanvas = GetComponent<Canvas>();
        }

        public void Active(bool state)
        {
            healthSlider.gameObject.SetActive(state);
        }

        public void HandleHealthMaxValue(int maxValue)
        {
            healthSlider.maxValue = maxValue;
        }

        public void HandleHealthChange(int currentHealth)
        {
            if (currentHealth < healthSlider.value)
            {
                DamageUiSignal();
            }
            
            unitHealth.text = currentHealth.ToString();
            healthSlider.value = currentHealth;
        }

        private void DamageUiSignal()
        {
            healthBackground.CrossFadeAlpha(0.2f, 0, false);
            healthBackground.CrossFadeAlpha(1, 0.8f, false);
        }

        public void HandleHealthSprite(string spriteName)
        {
            healthColor.sprite = _cacheSprites.GetSpriteFromAtlas
                ("HealthBar_" + spriteName, ItemInfo.Catalog.Setup);
        }

        public void HandleHealthActive(bool state)
        {
            _healthCanvas.enabled = state;
        }
    }
}
