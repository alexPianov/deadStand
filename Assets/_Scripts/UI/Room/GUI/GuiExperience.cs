using Cysharp.Threading.Tasks;
using EventBusSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel.UI
{
    public class GuiExperience : UiTransparency, IExperienceHandler
    {
        public TextMeshProUGUI userLevel;
        public TextMeshProUGUI userExperience;
        public Slider slider;
        private int maxExperience;
        private CanvasGroup _canvasGroup;

        [Inject]
        private Unit _unit;
        
        private async void Start()
        {
            EventBus.Subscribe(this);
            
            await UniTask.WaitUntil(() => _unit);
                
            _unit.Experience.UpdateLevelUi();
            _unit.Experience.UpdateMaxExperienceUi();
            _unit.Experience.UpdateExperienceUi();

            await UniTask.WaitUntil(() => сanvasGroup);
            
            Transparency(false);
        }
        
        private void OnDisable()
        {
            EventBus.Unsubscribe(this);
        }

        public void HandleLevelChange(int level)
        {
            userLevel.text = level + " Lvl";
        }
        
        public void HandleExperienceChange(int experienceAmount)
        {
            userExperience.text = experienceAmount + "/" + maxExperience;

            slider.maxValue = maxExperience;
            slider.value = experienceAmount;
        }

        public void HandleMaxExperience(int experienceAmount)
        {
            maxExperience = experienceAmount;
        }
    }
}
