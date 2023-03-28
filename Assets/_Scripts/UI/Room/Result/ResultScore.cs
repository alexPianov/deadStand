using Photon.Pun;
using TMPro;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class ResultScore : MonoBehaviourPunCallbacks
    {
        public TextMeshProUGUI CurrentScore;
        public TextMeshProUGUI BestScore;
        public ResultStars ResultStars;
        private int _countSpeed = 2000;
        
        [Inject] private Unit _unit;
        [Inject] private CacheAudio _cacheAudio;

        private void Start()
        {
            SetScoreValues();
        }

        private int currentScore;
        private int maxScore;
        private void SetScoreValues()
        {
            currentScore = StatHandler.GetRoundScore(_unit.photonView.Owner);
            
            if (currentScore == 1) currentScore = 0;
            
            maxScore = _unit.CacheUserInfo.payload
                .GetStatisticValue(UserPayload.Statistics.MaxMatchScore);

            BestScore.text = "Best Score: " + maxScore;

            scoreTimer = true;
            
            _cacheAudio.Play(CacheAudio.MenuSound.OnGetItem);
        }

        private bool scoreTimer;
        private float scoreValue;
        private void Update()
        {
            if(!scoreTimer) return;

            scoreValue += Time.deltaTime * _countSpeed;
            
            CurrentScore.text = "Score: " + Mathf.FloorToInt(scoreValue);

            if (currentScore > 0)
            {
                Star(0, 3);
                Star(1, 2);
                Star(2);
            }

            if (scoreValue >= currentScore)
            {
                scoreValue = 0;
                scoreTimer = false;
                
                CurrentScore.text = "Score: " + Mathf.FloorToInt(currentScore);
            }
        }

        private void Star(int number, int divider = 1)
        {
            if (scoreValue >= maxScore / divider && 
                !ResultStars.showedStarNumbers.Contains(number))
            {
                ResultStars.ShowStar(number);
                _cacheAudio.Play(CacheAudio.MenuSound.OnGetStar, true);
            }
        }
    }
}
