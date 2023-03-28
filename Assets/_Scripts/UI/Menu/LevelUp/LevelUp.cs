using TMPro;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class LevelUp : MonoBehaviour
    {
        public TextMeshProUGUI level;
        
        [Inject] private CacheUserInfo _cacheUserInfo;
        [Inject] private CacheAudio _cacheAudio;

        private void Start()
        {
            var levelPoints = _cacheUserInfo.payload
                .GetStatisticValue(UserPayload.Statistics.NewLevelPoints);
                
            var levelNumber = _cacheUserInfo.payload
                .GetStatisticValue(UserPayload.Statistics.Level);

            var levelSummary = levelNumber + levelPoints;

            level.text = levelSummary.ToString();
            
            _cacheAudio.Play(CacheAudio.MenuSound.OnNewLevel);
        }
    }
}