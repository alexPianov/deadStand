using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

namespace Playstel
{
    public static class PlayFabHandler
    {
        #region Cloud Scripts

        public enum Function
        {
            GetSessionItems, CreateDefaultPlayerData, SetCharacterToUser, 
            AddFriend, SetFriendTags, RemoveFriend, RedeemReferral, GrantItemsArray, 
            UpdateMainCurrency, CutsceneIsWatched
        }
        
        private static ExecuteCloudScriptResult cloudScriptResult;
        private static bool cloudScriptCompleted;
        public static async UniTask<ExecuteCloudScriptResult> ExecuteCloudScript
            (Function function, object args = null)
        {
            cloudScriptResult = null;
            cloudScriptCompleted = false;

            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = function.ToString(),
                FunctionParameter = args
            };
            
            PlayFabClientAPI.ExecuteCloudScript(
                request,
                result =>
                {
                    cloudScriptResult = result;
                    cloudScriptCompleted = true;
                },
                error =>
                {
                    Debug.LogError("Failed to execute: " + error.ErrorMessage);
                    cloudScriptCompleted = true;
                });

            await UniTask.WaitUntil(() => cloudScriptCompleted);
            
            return cloudScriptResult;
        }

        #endregion

        #region Statistics

        private static bool reportStatisticCompleted;
        public static async UniTask<UpdatePlayerStatisticsResult> ReportStatisticToServer
            (UserPayload.Statistics statisticName, int value = 0)
        {
            var Statistics = new List<StatisticUpdate>()
            {
                new StatisticUpdate()
                {
                    StatisticName = statisticName.ToString(),
                    Value = value
                }
            };

            return await ReportStatisticToServer(Statistics);
        }
        
        public static async UniTask<UpdatePlayerStatisticsResult> ReportStatisticToServer
            (List<StatisticUpdate> statisticUpdates)
        {
            UpdatePlayerStatisticsResult operationResult = null;
            reportStatisticCompleted = false;
        
            UpdatePlayerStatisticsRequest requestData = new UpdatePlayerStatisticsRequest() 
            {
                Statistics = statisticUpdates
            };
        
            PlayFabClientAPI.UpdatePlayerStatistics( requestData, result =>
            {
                operationResult = result;
                reportStatisticCompleted = true;

            }, error =>
            {
                Debug.Log( error.ErrorMessage );
                reportStatisticCompleted = true;
            } );

            await UniTask.WaitUntil(() => reportStatisticCompleted);
            return operationResult;
        }

        public static StatisticUpdate CreateStatistic(UserPayload.Statistics statistics, int value)
        {
            var statisticUpdates = new StatisticUpdate();
            statisticUpdates.StatisticName = statistics.ToString();
            statisticUpdates.Value = value;
            return statisticUpdates;
        }

        private static bool getLeaderboardSuccess;
        public static async UniTask<GetLeaderboardResult> GetLeaderboardData
            (UserPayload.Statistics statisticName, int maxResults = 99)
        {
            GetLeaderboardResult operationResult = null;
            getLeaderboardSuccess = false;
            var statName = statisticName.ToString();
            
            GetLeaderboardRequest redquestData = new GetLeaderboardRequest {
                StatisticName = statName,
                MaxResultsCount = maxResults,
                ProfileConstraints = new PlayerProfileViewConstraints()
                {
                    ShowDisplayName = true,
                    ShowLocations = true,
                    ShowStatistics = true
                }
            };

            GetLeaderboardRequest requestData = new GetLeaderboardRequest {
                StatisticName = statName,
                MaxResultsCount = maxResults,
                ProfileConstraints = new PlayerProfileViewConstraints()
                {
                    ShowDisplayName = true,
                    ShowLocations = true,
                    ShowStatistics = true
                }
            };
            
            PlayFabClientAPI.GetLeaderboard(requestData, result =>
            {
                operationResult = result;
                getLeaderboardSuccess = true;
            }, error =>
            {
                getLeaderboardSuccess = true;
            });
            
            await UniTask.WaitUntil(() => getLeaderboardSuccess);
            return operationResult;
        }

        #endregion

        #region Payload

        private static bool updatePayloadSuccess;
        public static async UniTask<GetPlayerCombinedInfoResult> GetUserPayload(string playFabId = null, 
            GetPlayerCombinedInfoRequestParams requestParams = null)
        {
            GetPlayerCombinedInfoResult payloadResult = null;
            updatePayloadSuccess = false;

            if (requestParams == null)
            {
                requestParams = PayloadAll();
            }
            
            var request = new GetPlayerCombinedInfoRequest
            {
                PlayFabId = playFabId,
                InfoRequestParameters = requestParams
            };

            PlayFabClientAPI.GetPlayerCombinedInfo(request, result =>
            {
                payloadResult = result;
                updatePayloadSuccess = true;
            }, error =>
            {
                updatePayloadSuccess = true;
            });

            await UniTask.WaitUntil(() => updatePayloadSuccess);

            return payloadResult;
        }

        public static GetPlayerCombinedInfoRequestParams PayloadAll()
        {
            var requestParams = new GetPlayerCombinedInfoRequestParams { };

            requestParams.GetUserAccountInfo = true;
            requestParams.GetUserData = true;
            requestParams.GetUserVirtualCurrency = true;
            requestParams.GetUserInventory = true;
            requestParams.GetPlayerProfile = true;
            requestParams.GetPlayerStatistics = true;

            return requestParams;
        }
        
        public static GetPlayerCombinedInfoRequestParams ExternalPayloadParamerets()
        {
            var requestParams = new GetPlayerCombinedInfoRequestParams { };

            requestParams.GetUserData = true;
            requestParams.GetPlayerProfile = true;
            requestParams.GetPlayerStatistics = true;

            return requestParams;
        }

        #endregion

        #region Account

        private static bool setTitleNameSuccess;
        public static async UniTask<UpdateUserTitleDisplayNameResult> SetTitleName
            (string userName, HandlerPulse pulse = null)
        {
            setTitleNameSuccess = false;
            UpdateUserTitleDisplayNameResult titleDisplayNameResult = null;
            
            PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
                    { DisplayName = userName },
                result =>
                {
                    Debug.Log("UpdateUserTitleDisplayName " + result.DisplayName);
                    titleDisplayNameResult = result;
                    setTitleNameSuccess = true; 
                },
                error =>
                {
                    Debug.Log("UpdateUserTitleDisplayName Error: " + error.ErrorMessage);
                    if(pulse) pulse.OpenTextNote(error.ErrorMessage);
                    setTitleNameSuccess = true;
                });

            await UniTask.WaitUntil(() => setTitleNameSuccess);
            return titleDisplayNameResult;
        }
        
        private static bool getRegistrationSuccess;
        public static async UniTask<RegisterPlayFabUserResult> RegistrationRequest
            (string inputName, string inputPass, string inputMail)
        {
            if (inputName == null || inputPass == null || inputMail == null) return null;

            getRegistrationSuccess = false;
            RegisterPlayFabUserResult operationResult = null;

            var RegisterRequest = new RegisterPlayFabUserRequest
            {
                Username = inputName,
                Password = inputPass,
                Email = inputMail
            };

            PlayFabClientAPI.RegisterPlayFabUser(RegisterRequest, result =>
                {
                    operationResult = result;
                    getRegistrationSuccess = true;
                }, error =>
                {
                    getRegistrationSuccess = true;
                });
            
            await UniTask.WaitUntil(() => getRegistrationSuccess);
            return operationResult;
        }
        
        private static bool getPhotonTokenSuccess;
        public static async UniTask<string> GetPhotonToken(string photonAppId)
        {
            Debug.Log("GetPhotonToken: " + photonAppId);
            
            getPhotonTokenSuccess = false;
            GetPhotonAuthenticationTokenResult photonTokenResult = null;
            
            PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest
                    { PhotonApplicationId = photonAppId },
                result =>
                {
                    photonTokenResult = result;
                    getPhotonTokenSuccess = true; 
                },
                error =>
                {
                    Debug.Log("UpdateUserTitleDisplayName Error: " + error.ErrorMessage);
                    getPhotonTokenSuccess = true;
                });

            await UniTask.WaitUntil(() => getPhotonTokenSuccess);
            return photonTokenResult.PhotonCustomAuthenticationToken;
        }

        #endregion

        #region Friendlist

        private static bool getFriendListSuccess;
        public async static UniTask<List<FriendInfo>> GetFriendList()
        {
            getFriendListSuccess = false;
            List<FriendInfo> friendInfoList = new();
            
            PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
            {
                IncludeSteamFriends = false,
                IncludeFacebookFriends = true,
                XboxToken = null,
                ProfileConstraints = new PlayerProfileViewConstraints
                {
                    ShowDisplayName = true,
                    ShowLastLogin = true,
                    ShowLocations = true,
                    ShowStatistics = true,
                }
            }, result =>
            {
                friendInfoList = result.Friends;
                getFriendListSuccess = true;
            }, error =>
            {
                Debug.LogError("Get Friends List Error: " + error.ErrorMessage);
                getFriendListSuccess = true;
            });

            await UniTask.WaitUntil(() => getFriendListSuccess);

            return friendInfoList;
        }
        
        
        private static bool friendAddSuccess;
        public async static UniTask<AddFriendResult> AddFriend(string friendPlayFabId)
        {
            friendAddSuccess = false;
            AddFriendResult addFriendResult = new();
            
            PlayFabClientAPI.AddFriend(new AddFriendRequest
            {
                FriendPlayFabId = friendPlayFabId

            }, result =>
            {
                addFriendResult = result;
                friendAddSuccess = true;
            }, error =>
            {
                friendAddSuccess = true;
            });
            
            await UniTask.WaitUntil(() => friendAddSuccess);

            return addFriendResult;
        }
        
        
        private static bool friendRemoveSuccess;
        public async static UniTask<RemoveFriendResult> RemoveFriend(string friendPlayFabId)
        {
            friendRemoveSuccess = false;
            RemoveFriendResult removeFriendResult = new();
            
            PlayFabClientAPI.RemoveFriend(new RemoveFriendRequest
            {
                FriendPlayFabId = friendPlayFabId

            }, result =>
            {
                removeFriendResult = result;
                friendRemoveSuccess = true;
            }, error =>
            {
                friendRemoveSuccess = true;
            });
            
            await UniTask.WaitUntil(() => friendRemoveSuccess);

            return removeFriendResult;
        }
        
        #endregion

        #region Title

        private static bool getTitleDataSuccess;
        public async static UniTask<GetTitleDataResult> GetTitleData()
        {
            getTitleDataSuccess = false;
            
            GetTitleDataResult titleDataResult = new();
            
            PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
                result =>
                {
                    titleDataResult = result;
                    getTitleDataSuccess = true;
                },
                error =>
                {
                    getTitleDataSuccess = true;
                }
            );
            
            await UniTask.WaitUntil(() => getTitleDataSuccess);

            return titleDataResult;
        }
        

        #endregion
    }
}
