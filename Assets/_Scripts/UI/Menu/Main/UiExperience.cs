using TMPro;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiExperience : UiTransparency
    {
        public TextMeshProUGUI userLevel;
        public TextMeshProUGUI userExperience;
        public Slider slider;

        [Inject] private CacheUserInfo _cacheUserInfo;

        private void Start()
        {
            UpdateExperienceDisplay();
            Transparency(false);
        }

        public void UpdateExperienceDisplay()
        {
            var level = _cacheUserInfo.payload.GetStatisticValue(UserPayload.Statistics.Level);
            SetLevel(level);
            
            var maxExp = _cacheUserInfo.payload.GetStatisticValue(UserPayload.Statistics.MaxExperience);
            var exp = _cacheUserInfo.payload.GetStatisticValue(UserPayload.Statistics.Experience);
            
            SetExperience(exp, maxExp);
        }

        private void SetLevel(int level)
        {
            userLevel.text = level + " Lvl";
        }

        private void SetExperience(int experienceAmount, int maxExperience)
        {
            userExperience.text = experienceAmount + "/" + maxExperience;

            slider.maxValue = maxExperience;
            slider.value = experienceAmount;
        }
    }
}