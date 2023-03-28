using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using Playstel.Bootstrap;
using TMPro;
using UnityEngine.SceneManagement;
using Region = Photon.Realtime.Region;

namespace Playstel
{
	public class ConnectPhoton: MonoBehaviourPunCallbacks
	{
		public List<Region> regionsList;
		public CacheUserSettings userSettings;
		public UserPayload userPayload;

		private string _cloudRegion;

		[Header("UI Handlers")]
        public HandlerNetworkError HandlerNetworkError;
        public HandlerPulse HandlerPulse;

		private bool _isConnected;

		public async UniTask SetPlayFabAuthValues(PlayFabAuthenticationContext authenticationContext)
        {
            PhotonNetwork.AuthValues = null;
			
            var photonAppId = GetPhotonAppId();
			
            GetPhotonAuthenticationTokenRequest request = new GetPhotonAuthenticationTokenRequest();

            request.PhotonApplicationId = photonAppId;
            request.AuthenticationContext = authenticationContext;

            PlayFabClientAPI.GetPhotonAuthenticationToken(request,
                result =>
                {
                    var photonAuthTokenFromPlayFab = result.PhotonCustomAuthenticationToken;

                    var photonUserId = userPayload.GetPlayFabId();
                    
                    AuthenticationValues cutomAuth = new AuthenticationValues();
                    cutomAuth.AuthType = CustomAuthenticationType.Custom;
                    cutomAuth.AddAuthParameter("username", photonUserId);
                    cutomAuth.AddAuthParameter("token", photonAuthTokenFromPlayFab);
                    cutomAuth.UserId = photonUserId;
					
                    PhotonNetwork.NetworkingClient.AppId = photonAppId;
                    PhotonNetwork.NickName = userPayload.GetTitleDisplayName();
                    PhotonNetwork.KeepAliveInBackground = 360;

                    PhotonNetwork.AuthValues = cutomAuth;
                },
                error =>
                {
                    Debug.Log("UpdateUserTitleDisplayName Error: " + error.ErrorMessage);
                });
            
            await UniTask.WaitUntil(() => PhotonNetwork.AuthValues != null);
            
            Debug.Log("--- Photon auth values is updated ---");
        }
        
        private string GetPhotonAppId()
        {
            var photonAppId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;
			
            if (string.IsNullOrEmpty(photonAppId) || string.IsNullOrWhiteSpace(photonAppId))
            {
                photonAppId = userSettings.GetPhotonRealtimeAppId();
                PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = photonAppId;
            }

            return photonAppId;
        }

        public void ClearRoomCache()
        {
	        PhotonHandler.ClearNetworkCustomProperties(PhotonNetwork.LocalPlayer);
	        PhotonNetwork.OpRemoveCompleteCacheOfPlayer(PhotonNetwork.LocalPlayer.ActorNumber);
        }
        
		public async UniTask Connect(string region = null, bool cancelRecentConnection = false)
		{
			if (PhotonNetwork.InRoom)
			{
				Debug.Log("Edit connection settings in the room is restricted");
				return;
			}

			_isConnected = false;
			HandlerNetworkError.StartTimer(true);

			if (cancelRecentConnection)
			{
				PhotonNetwork.Disconnect();
			}
			else
			{
				if (PhotonNetwork.IsConnectedAndReady)
				{
					Debug.Log("Already connected to master and ready to join and create rooms");
					_isConnected = true;
					HandlerNetworkError.StartTimer(false);
					return;
				}
			}

			if (string.IsNullOrEmpty(region))
			{
				await GetFixedRegion();
			}
			else
			{
				var result = await ConnectToFixedRegion(region);
				
				if (!result)
				{
					await GetFixedRegion();
				}
			}

			await UniTask.WaitUntil(() => _isConnected);
		}

		private bool gettingFixedRegion;
		private async UniTask GetFixedRegion()
		{
			Debug.Log("Connect using settings to get fixed region");

			gettingFixedRegion = true;
			
			var result = PhotonNetwork.ConnectUsingSettings();
			
			if (!result)
			{
				Debug.LogError("Photon Network | Connect Using Settings failed on getting fixed region");
			}
		}

		public override void OnRegionListReceived(RegionHandler regionHandler)
		{
			gettingFixedRegion = false;
			regionHandler.PingMinimumOfRegions(OnRegionsPinged, null);
		}

		private void OnRegionsPinged(RegionHandler regionHandler)
		{
			regionsList = regionHandler.EnabledRegions.OrderBy(x => x.Ping).ToList();
			
			Debug.Log("Available regions count: " + regionsList.Count);
			
			PhotonNetwork.Disconnect();

			ConnectToFixedRegion(regionsList[0].Code);
		}

		private string currentRegion;
		public async UniTask<bool> ConnectToFixedRegion(string region)
		{
			Debug.Log("ConnectToFixedRegion: " + region);

			if(string.IsNullOrEmpty(region)) return false;

			currentRegion = region;
			
			PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = region;

			Debug.Log("Wait for photon auth values");
			await UniTask.WaitUntil(() => PhotonNetwork.AuthValues != null);
			Debug.Log("Photon auth values is get");

			var result = PhotonNetwork.ConnectUsingSettings();

			if (!result)
			{
				Debug.LogError("Photon Network | Connect Using Settings failed on connecting to fixed region: " + region);
			}

			return result;
		}

		public override void OnConnectedToMaster()
		{
			if(gettingFixedRegion) return;
			
			Debug.Log("Connected to " + PhotonNetwork.Server);

			JoinLobby();
		}

		private void JoinLobby()
		{
			bool result = PhotonNetwork.JoinLobby();

			if (result)
			{
				_isConnected = true;
				HandlerNetworkError.StartTimer(false);
				userSettings.SetRegion(currentRegion);
			}
			else
			{
				Debug.LogError("Failed to connect to cloud region: " + PhotonNetwork.CloudRegion);
				_isConnected = true;
				HandlerPulse.OpenTextNote("Failed to connect to cloud region: " + PhotonNetwork.CloudRegion);
				userSettings.pickedRegion = null;
			}
		}

		public async override void OnDisconnected(DisconnectCause cause)
		{
			Debug.Log("Disconnecting: " + cause);
			
			if (!_isConnected) return;
			_isConnected = false;
			
			if(PhotonNetwork.InRoom) return;

			if (cause == DisconnectCause.DisconnectByServerLogic) return; 
			
			if (cause == DisconnectCause.DisconnectByClientLogic) return; 

			await UniTask.Delay(3000);

			await Connect(userSettings.pickedRegion);

			if (PhotonNetwork.IsConnectedAndReady)
			{
				Debug.Log("--- Connection Restored ---");
			}
		}
		
		public override void OnLeftLobby()
		{
			Debug.Log("Left Lobby");
			//NetworkError();
		}

		private void NetworkError()
		{
			if (!_isConnected) return;
			_isConnected = false;
			
			Debug.LogError("Photon Network Error");

			HandlerNetworkError.StartTimer(false);
			HandlerNetworkError.ActivePanel(true);
		}

		public void OnApplicationQuit()
		{
			PhotonNetwork.Disconnect();
		}
	}
}
