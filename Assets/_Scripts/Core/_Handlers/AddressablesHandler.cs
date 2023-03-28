using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace Playstel
{
    public static class AddressablesHandler 
    {
        public static async UniTask<T> Load<T>(string assetName)
        {
            var asset = await Addressables.LoadAssetAsync<T>(assetName);
            if (asset == null) {Debug.Log("Asset is not found: " + assetName); return default;}
            return asset;
        }

        public static async UniTask InstallSeason(string seasinName, 
            TextMeshProUGUI processText = null, bool deleteSceneCache = false)
        {
            Debug.Log("InstallExternalScene: " + seasinName);

            var sceneName = "Season_" + seasinName;
            
            if (deleteSceneCache)
            {
                Addressables.ClearDependencyCacheAsync(sceneName);
                await UniTask.Delay(1000);
            }
            
            if (processText)
            {
                processText.text = "Install " + seasinName + " Season";
            }
            
            await Addressables.DownloadDependenciesAsync(sceneName);
        }

        public static async UniTask InstallAssets<T>(string label, 
            TextMeshProUGUI processText = null, Slider slider = null)
        {
            var resources = await Addressables
                .LoadResourceLocationsAsync(label);
            
            Debug.Log("InstallAssets " + resources.Count + " from label: " + label);

            if (slider)
            {
                slider.value = 0;
                slider.maxValue = resources.Count;
            }

            if (processText)
            {
                processText.text = "Install " + label;
            }
            
            foreach (var resource in resources)
            {
                if (slider) slider.value++;
                await Addressables.LoadAssetAsync<T>(resource);
            }
        }

        private static string GetAssetName(string internalId)
        {
            var list = internalId.Split('/');
            var assetName = list[list.Length - 1];
            var assetNameShort = assetName.Split('.');
            return assetNameShort[0];
        }

        public static async UniTask<List<T>> LoadLabelAssets<T>(IList<string> labelName)
        {
            var assetList = new List<T>();

            var resources = await Addressables.LoadResourceLocationsAsync
                (labelName, Addressables.MergeMode.Union).Task;

            foreach (var resource in resources)
            {
                var asset = await Addressables.LoadAssetAsync<T>(resource);
                assetList.Add(asset);
            }

            return assetList;
        }

        public static async UniTask<GameObject> Get(string assetName, Transform parent = null)
        {
            if (string.IsNullOrEmpty(assetName)) return null;
            
            var asset = await Addressables.InstantiateAsync(assetName, parent);
            
            ReleaseFromMemoryOnDestroy(asset);
            
            return asset;
        }

        public static void ReleaseFromMemoryOnDestroy(GameObject assetInstance)
        {
            if (!assetInstance) return;

            if (assetInstance.GetComponent<NotifyOnDestroy>())
            {
                var notify = assetInstance.GetComponent<NotifyOnDestroy>();
                notify.instanceRef = assetInstance;
                notify.Destroyed += ReleaseFromMemory;
            }
            else
            {
                var notify = assetInstance.AddComponent<NotifyOnDestroy>();
                notify.instanceRef = assetInstance;
                notify.Destroyed += ReleaseFromMemory;
            }
        }

        private static void ReleaseFromMemory(GameObject assetReference, NotifyOnDestroy instance)
        {
            assetReference.SetActive(false);
            var result = Addressables.ReleaseInstance(assetReference);
        }
        
        
        private static bool _addressablesIsLoad;
        public static async UniTask InstallAddressables()
        {
            Addressables.InitializeAsync().Completed += result => 
            {
                _addressablesIsLoad = true;
            };
			
            await UniTask.WaitUntil(() => _addressablesIsLoad);
        }

        public static void LoadAddressableItemsToCache(CacheItemInfo cacheItemInfo)
        {
            var itemsToLoad = new List<ItemInfo>();
			
            itemsToLoad.AddRange(cacheItemInfo
                .GetItemInfoList(ItemInfo.Catalog.Character, ItemInfo.Class.Stuff));

            itemsToLoad.AddRange(cacheItemInfo
                .GetItemInfoList(ItemInfo.Catalog.Weapons, ItemInfo.Class.Firearm));
			
            foreach (var item in itemsToLoad)
            {
                if(item == null) continue;
                if(item.itemName == null) continue;
                Load<GameObject>(item.itemName);
            }
        }
    }
}
