using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EventBusSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Playstel
{
    public class BootstrapAddressables : MonoBehaviour
    {
	    public bool isFinished;
	    public HandlerLoading HandlerLoading;
	    public CacheUserSettings UserSettings;
	    public CacheVideo CacheVideo;
	    public GameObject installAttention;
	    
		private List<string> _preloadLabels = new List<string>() 
			{ "UI", "Materials", "Firearm", "Character" };
		
		public async UniTask InstallCheck(bool reinstallCore = false)
		{
			if (isFinished) return;

			if (reinstallCore)
			{
				UserSettings.SetCoreInstall(false);
			}
			
			await AddressablesHandler.InstallAddressables();

			if (!UserSettings.CoreInstalled)
			{
				installAttention.SetActive(true);
			}
			else
			{
				isFinished = true;
			}

			await UniTask.WaitUntil(() => isFinished);
		}

		public async void StartInstall()
		{
			installAttention.SetActive(false);

			PlaySeasonTrailer();
			
			HandlerLoading.ActiveLoadingText(true);
			HandlerLoading.ActiveLoadingSlider(true);

			await InstallSeason(UserSettings.pickedSeason);
			await InstallCore();
			
			HandlerLoading.ActiveLoadingSlider(false);
			
			UserSettings.SetCoreInstall(true);
			
			isFinished = true;
		}

		private async void PlaySeasonTrailer()
		{
			DisableBootUi(true);
			await CacheVideo.PlayVideo(UserSettings.pickedSeason);
			DisableBootUi(false);
			HandlerLoading.ActiveLoadingSlider(true);
		}
		
		private void DisableBootUi(bool state)
		{
			Debug.Log("DisableBootUi: " + state);
			HandlerLoading.ActiveInfoPanel(!state);
			EventBus.RaiseEvent<IBootCanvasHandler>(h => h.HandleCanvasTransparency(state));
		}

		[ContextMenu("Clear Cache")]
		public void ClearCache()
		{
			Addressables.CleanBundleCache().Completed += handle => { Debug.Log("Bundle Cache is Cleared");};
			Addressables.ClearResourceLocators();
			Debug.Log("Resource Locators is Cleared");
		}

		private async UniTask InstallCore()
		{
			foreach (var label in _preloadLabels)
			{
				Debug.Log("Preload label: " + label);
				
				if (label == "Mesh")
				{
					await AddressablesHandler.InstallAssets<Mesh>
						(label, HandlerLoading.detailsText, HandlerLoading.installSlider);

					continue;
				}

				if (label == "Materials")
				{
					await AddressablesHandler.InstallAssets<Material>
						(label, HandlerLoading.detailsText, HandlerLoading.installSlider);

					continue;
				}

				if (label == "Soundtrack")
				{
					await AddressablesHandler.InstallAssets<AudioClip>
						(label, HandlerLoading.detailsText, HandlerLoading.installSlider);

					continue;
				}

				if (label == "SFX")
				{
					await AddressablesHandler.InstallAssets<AudioClip>
						(label, HandlerLoading.detailsText, HandlerLoading.installSlider);

					continue;
				}

				await AddressablesHandler.InstallAssets<GameObject>
					(label, HandlerLoading.detailsText, HandlerLoading.installSlider);
			}
		}

		public async UniTask InstallSeason(string seasinName, float estimatingMinutes = 10f)
		{
			HandlerLoading.ActiveSeasonSlider(true, estimatingMinutes);
			await AddressablesHandler.InstallSeason(seasinName, HandlerLoading.detailsText);
			HandlerLoading.ActiveSeasonSlider(false);
		}
    }
}