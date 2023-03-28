using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class ResultSave : MonoBehaviourPunCallbacks
    {
        [Inject] private Unit _unit;
        [Inject] private HandlerLoading _handlerLoading;
        public async void LeaveRoom()
        {
            _handlerLoading.OpenLoadingScreen(true, HandlerLoading.Text.Saving);
            _handlerLoading.ActiveLoadingText(true);
            _handlerLoading.SetLoadingText("Save Progress");

            await PlayFabHandler.ReportStatisticToServer(GetStatisticUpdate());
            
            PhotonNetwork.LeaveRoom();
        }

        private List<StatisticUpdate> GetStatisticUpdate()
        {
            var statisticUpdates = new List<StatisticUpdate>();

            statisticUpdates.Add(CreateStatistic
                (UserPayload.Statistics.Frags, StatHandler.GetFrags(_unit.photonView.Owner)));

            statisticUpdates.Add(CreateStatistic
                (UserPayload.Statistics.Deaths, StatHandler.GetDeaths(_unit.photonView.Owner)));
            
            statisticUpdates.Add(CreateStatistic
                (UserPayload.Statistics.Experience, StatHandler.GetExperience(_unit.photonView.Owner)));
            
            statisticUpdates.Add(CreateStatistic
                (UserPayload.Statistics.NewLevelPoints, GetNewLevelPoints()));

            statisticUpdates.Add(CreateStatistic
                (UserPayload.Statistics.MaxMatchScore, StatHandler.GetRoundScore(_unit.photonView.Owner)));

            statisticUpdates.Add(CreateStatistic
                (UserPayload.Statistics.GameTime, _unit.PlayTime.GetMinutes()));

            statisticUpdates.Add(CreateStatistic
                (UserPayload.Statistics.Matches));

            return statisticUpdates;
        }

        private StatisticUpdate CreateStatistic(UserPayload.Statistics statistics, int value = 1)
        {
            return PlayFabHandler.CreateStatistic(statistics, value);
        }
        
        private int GetNewLevelPoints()
        {
            var recentLevel = _unit.CacheUserInfo.payload
                .GetStatisticValue(UserPayload.Statistics.Level);
            
            var currentLevel = StatHandler.GetLevel(_unit.photonView.Owner);
            
            Debug.Log("GetNewLevelPoints | recentLevel: " + recentLevel + " currentLevel: " + currentLevel);
            
            if (currentLevel > recentLevel)
            {
                return currentLevel - recentLevel;
            }

            return 0;
        }
    }
}
