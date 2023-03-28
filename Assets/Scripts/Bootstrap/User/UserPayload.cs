using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using PlayFab.ClientModels;
using UnityEngine;

namespace Playstel
{
    [CreateAssetMenu(menuName = "User/Payload")]
    public class UserPayload : ScriptableObject
    {
        [Header("User Data")]
        public UserData userData;
        public UserProfile userProfile;

        private GetPlayerCombinedInfoResultPayload playfabPayload;
        private ItemInfo.Currency mainCurrency = ItemInfo.Currency.GL;

        public async UniTask<GetPlayerCombinedInfoResultPayload> UpdatePayload(string playFabId = null, 
            GetPlayerCombinedInfoRequestParams requestParams = null)
        {
            Debug.Log("UpdatePayload " + playFabId);

            if (GetPlayFabPayload() != null && !string.IsNullOrEmpty(GetPlayFabId()))
            {
                playFabId = GetPlayFabId();
            }

            if (string.IsNullOrEmpty(playFabId))
            {
                Debug.LogError("PlayFabId is empty");
                return null;
            }
            
            var result = await PlayFabHandler.GetUserPayload(playFabId, requestParams);

            if (result == null)
            {
                Debug.LogError("Failed to load uset payload");
            }
            else
            {
                await ReceivePayload(result.InfoResultPayload);
            }
            
            return playfabPayload;
        }

        public async UniTask ReceivePayload(GetPlayerCombinedInfoResultPayload payload)
        {
            if (payload == null)
            {
                Debug.LogError("Failed to find playfab payload result");  return;
            } 
            
            playfabPayload = payload;

            userData.UpdateUserDataRecord(playfabPayload.UserData);
            
            await UpdateUserProfile();
        }
        
        private async UniTask UpdateUserProfile()
        {
            var profileResult = await userProfile.GetUserProfile(this);
            if(profileResult == null) return;
            playfabPayload.PlayerProfile = profileResult.PlayerProfile;
        }

        #region Handled
        
        public GetPlayerCombinedInfoResultPayload GetPlayFabPayload()
        {
            return playfabPayload;
        }
        
        public string GetPlayFabId()
        {
            return playfabPayload.AccountInfo.PlayFabId;
        }
        
        public ObscuredInt GetPlayerMainCurrency()
        {
            playfabPayload.UserVirtualCurrency.TryGetValue(mainCurrency.ToString(), out var value);
            return value;
        }

        public string GetTitleDisplayName()
        {
            return playfabPayload.AccountInfo.TitleInfo.DisplayName;
        }

        public string GetCountryCode()
        {
            if (playfabPayload.PlayerProfile == null)
            {
                Debug.LogError("Failed to find Player Profile");
                return null;
            }
            
            var locations = playfabPayload.PlayerProfile.Locations;

            if (locations == null || locations[0] == null) return null;
            
            return locations[0].CountryCode.ToString().ToLower();
        }
        
        public enum Statistics
        {
            Level, Experience, MaxExperience, 
            Matches, MaxMatchScore, 
            Frags, Deaths, GameTime,
            NewLevelPoints, AnarchySeasonPoints
        }

        public int GetStatisticValue(Statistics statisticName)
        {
            return GetStatisticValue(statisticName.ToString());
        }
        
        public int GetStatisticValue(string statisticName)
        {
            var stat = playfabPayload.PlayerStatistics
                .Find(info => info.StatisticName == statisticName);

            if(stat == null)
            {
                Debug.Log("Failed to find statistic string: " + statisticName);
                return 0;
            }

            return stat.Value;
        }
        
        #endregion
    }
}
