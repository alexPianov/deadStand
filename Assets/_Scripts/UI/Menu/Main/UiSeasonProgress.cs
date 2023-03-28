using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiSeasonProgress : MonoBehaviour
    {
        public TextMeshProUGUI seasonName;
        public TextMeshProUGUI seasonProgress;
        public Slider progressSlider;

        private int userSeasonPoints;
        private int maxSeasonPoints;

        [Inject] private CacheUserSettings _cacheUserSettings;
        [Inject] private CacheTitleData _cacheTitleData;
        [Inject] private CacheUserInfo _cacheUserInfo;

        private void Start()
        {
            seasonName.text = _cacheUserSettings.pickedSeason;

            if (_cacheUserSettings.pickedSeason == SeasonSlot.Season.Anarchy.ToString())
            {
                userSeasonPoints = _cacheUserInfo.payload
                    .GetStatisticValue(UserPayload.Statistics.AnarchySeasonPoints);

                maxSeasonPoints = _cacheTitleData
                    .GetTitleDataInt(CacheTitleData.TitleDataKey.AnarchySeasonLevels);
            }

            seasonProgress.text = userSeasonPoints.ToString();
            progressSlider.value = userSeasonPoints;
            progressSlider.maxValue = maxSeasonPoints;
        }
    }
}