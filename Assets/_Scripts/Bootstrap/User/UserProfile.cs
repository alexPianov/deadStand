using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Models;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Playstel
{
    [CreateAssetMenu(menuName = "User/Profile")]
    public class UserProfile : ScriptableObject
    {
        public async UniTask<GetPlayerProfileResult> GetUserProfile(UserPayload payload)
        {
            _result = null;

            var request = new GetPlayerProfileRequest
            {
                PlayFabId = payload.GetPlayFabId(),
                
                ProfileConstraints = new PlayerProfileViewConstraints 
                {
                    ShowDisplayName = true,
                    ShowStatistics = true,
                    ShowLocations = true
                }
            };

            PlayFabClientAPI.GetPlayerProfile(request, GetPlayerProfileResult, OnPlayFabError);
            
            await UniTask.WaitUntil(() => _result != null);

            return _result;
        }

        private GetPlayerProfileResult _result;
        private void GetPlayerProfileResult(GetPlayerProfileResult result)
        {
            _result = result;
        }
        
        public PlayerProfileViewConstraints FriendViewConstraints()
        {
            var constraints = new PlayerProfileViewConstraints { };

            constraints.ShowDisplayName = true;
            constraints.ShowStatistics = true;
            constraints.ShowLocations = true;
            constraints.ShowLastLogin = true;

            return constraints;
        }

        private void OnPlayFabError(PlayFabError obj)
        {
            Debug.LogError(obj.GenerateErrorReport());
        }
    }
}
