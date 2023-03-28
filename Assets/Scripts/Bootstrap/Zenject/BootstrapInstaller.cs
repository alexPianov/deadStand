using System;
using Cysharp.Threading.Tasks;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Playstel.Bootstrap
{
	public class BootstrapInstaller : MonoInstaller
	{
		[Header("Content")] 
		public BootstrapAddressables Addressables;
		public bool reinstallCore;
		
        [Header("Handlers")]
        public HandlerLoading loading;
        public HandlerPulse handlerPulse;
        public HandlerNetworkError handlerNetworkError;

        [Header("Network")]
		public ConnectPhoton connectPhoton;
		public ConnectPlayFab connectPlayFab;
        public ConnectRoom connectRoom;

        [Header("Cache")]
		public CacheAudio cacheAudio;
		public CacheVideo cacheVideo;
		public CacheSoundClips cacheSoundClips;
		public CacheItemInfo cacheItemInfo;
		public CacheMesh cacheMesh;
		public CacheSprites cacheSprites; 
		public CacheUserInfo cacheUserInfo;
		public CacheGizmos cacheGizmos;
		public CacheTitleData cacheTitleData;
		public CacheRatingList cacheRatingList;
		public CacheUserFriends cacheUserFriends;
		public CacheUserSettings cacheUserSettings;
		public CacheBoosters cacheBoosters;

		[Header("User")]
		public UserInventory userInventory;
		
		[Header("Post Processing")] 
		public Volume volume;

		[Header("Start Button")] 
		public UiTransparency startButtonTransparency;
		public Button startButton;

		public override void InstallBindings()
		{
			BindAll();
		}

		public void Start()
		{
			startButton.onClick.AddListener(StartGame);
			startButton.gameObject.SetActive(false);
			
			cacheUserSettings.SetPostProcessingVolume(volume);
			cacheUserSettings.LoadSettings();
			
			cacheVideo.InstallStartVideo(cacheUserSettings.pickedSeason);
			cacheAudio.PlayMusic(CacheAudio.Music.Loading);

			DataBoot();
		}

		[ContextMenu("Clear Prefs")]
		public void ClearPrefs()
		{
			PlayerPrefs.DeleteAll();
		}
		
		public async void DataBoot(bool enableStartButton = true)
		{
			Debug.Log("--- Data Boot ---");
			
			cacheUserSettings.DisableAudioMaster(false);
			cacheAudio.ActiveAudioListener(true);
			loading.ActiveLoadingText(true);

			await LoadBootScene();
			
			await connectPlayFab.LoginByPrefs();
			
			if (connectPlayFab.CurrentLoginStatus == ConnectPlayFab.LoginStatus.Failed)
			{
				Debug.Log("Data Boot Cancel - Login Failed");
				return;
			}
			
			if (connectPlayFab.NewlyCreated.NewlyStatus())
			{
				Debug.Log("Data Boot Cancel - User is new");
				return;
			}

			loading.SetLoadingText("Install Addressables");
			await Addressables.InstallCheck(reinstallCore);

			loading.SetLoadingText("Realtime Network");
			await connectPhoton.Connect(cacheUserSettings.pickedRegion);
			connectPhoton.ClearRoomCache();
			
			await InstallCacheData();

			loading.SetLoadingText("Inventory Update");
			await userInventory.Install(cacheItemInfo);

			loading.SetLoadingText("Friends Update");
			await cacheUserFriends.Install();

			loading.SetLoadingText("Wait for clip ending");
			await UniTask.WaitUntil(() => !cacheVideo.VideoPlayer.clip);

			loading.SetLoadingText(null);
			loading.ActiveLoadingText(false);
			loading.ActiveLoadingSlider(false);
			
			if (enableStartButton)
			{
				startButton.interactable = true;
				startButton.gameObject.SetActive(true);
				startButtonTransparency.Transparency(false);
			}
			else
			{
				StartGame();
			}
		}

		private async void StartGame()
		{
			Debug.Log("StartGame");
			startButton.interactable = false;
			
			cacheAudio.PlayMusic(CacheAudio.Music.Null);
			cacheAudio.Play(CacheAudio.MenuSound.OnSplash);

			loading.OpenBlackScreen(true);
			
			await UniTask.Delay(200);
			
			startButton.gameObject.SetActive(false);

			LoadNextScene();
		}
		
		private bool cacheIsLoaded;
		private async UniTask InstallCacheData()
		{
			if(cacheIsLoaded) return;
			
			loading.SetLoadingText("Get Sprites");
			await cacheSprites.install.Install();
			
			loading.SetLoadingText("Get Item Info");
			await cacheItemInfo.install.Install(cacheSprites);
			
			loading.SetLoadingText("Get Mesh");
			await cacheMesh.install.Install();
			
			loading.SetLoadingText("Get Gizmos");
			await cacheGizmos.install.Install();
			
			loading.SetLoadingText("Get Title Data");
			await cacheTitleData.InstallTitleData();

			loading.SetLoadingText("Get Room Settings");
			await connectRoom.Install(cacheItemInfo);
			
			loading.SetLoadingText("Get Sounds");
			await cacheSoundClips.CacheRoundSoundtracks(cacheUserSettings.pickedSeason);
			await cacheSoundClips.CacheHitSounds();
			await cacheSoundClips.CacheReloadSounds();

			cacheIsLoaded = true;
		}
		
		public async UniTask LoadBootScene()
		{
			await SceneHandler.LoadAsync(SceneHandler.Scenes.Boot);
		}

		private async void LoadNextScene()
		{
			Debug.Log("LoadNextScene");
			cacheAudio.PlayMusic(CacheAudio.Music.Menu);

			var sessionItems = await HaveSessionItems();
			
			if (sessionItems)
			{
				Debug.Log("--- LOAD NEXT SCENE " + SceneHandler.Scenes.Menu + " ---");

				await SceneHandler.LoadAsync(SceneHandler.Scenes.Menu);
			}
			else
			{
				Debug.Log("--- LOAD NEXT SCENE " + SceneHandler.Scenes.CreateCharacter + " ---");

				await SceneHandler.LoadAsync(SceneHandler.Scenes.CreateCharacter);
			}
			
			await UniTask.Delay(1400);
			
			loading.OpenBlackScreen(false);
		}

		private async UniTask<bool> HaveSessionItems()
		{
			var result = await PlayFabHandler.ExecuteCloudScript
				(PlayFabHandler.Function.GetSessionItems);

			if (result == null) return false;
			if (result.FunctionResult == null) return false;

			var items = PlayFabSimpleJson
				.DeserializeObject<string[]>(result.FunctionResult.ToString());
			
			if (items == null || items.Length == 0) return false;
			
			return true;
		}
		
		private void BindAll()
		{
			BindFrom(this);
			
			BindFrom(loading);
			BindFrom(handlerPulse);
			BindFrom(handlerNetworkError);

			BindFrom(connectPhoton);
			BindFrom(connectPlayFab);
			BindFrom(connectRoom);

			BindFrom(cacheAudio);
			BindFrom(cacheVideo);
			BindFrom(cacheSoundClips);
			BindFrom(cacheItemInfo);
			BindFrom(cacheMesh);
			BindFrom(cacheSprites);
			BindFrom(cacheUserInfo);
			BindFrom(cacheGizmos);
			BindFrom(cacheTitleData);
			BindFrom(cacheRatingList);
			BindFrom(cacheUserFriends);
			BindFrom(cacheUserSettings);
			BindFrom(cacheBoosters);
			
			BindFrom(volume);
		}

		private void BindFrom<T>(T instance)
		{
			Container.Bind<T>().FromInstance(instance).AsSingle().NonLazy();
		}

	}
}
