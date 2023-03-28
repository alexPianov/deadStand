using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using PlayFab.ClientModels;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiCharacterSaveMaster : MonoBehaviour
    {
        [Inject] private Unit _unit;
        [Inject] private LocationInstaller _locationInstaller;
        [Inject] private HandlerLoading _handlerLoading;
        [Inject] private CacheUserInfo _cacheUserInfo;
        [Inject] private CacheItemInfo _cacheItemInfo;

        public CatalogItem PickedCharacter;
        private int updateTimeMSec = 2000;
        
        public void Start()
        {
            _locationInstaller.BindFromInstance(this);
        }
        
        public void SetPickedCharacter(CatalogItem item)
        {
            PickedCharacter = item;
        }

        public CatalogItem GetPickedCharacter()
        {
            return PickedCharacter;
        }

        public async UniTask SaveCharacter()
        {
            if (PickedCharacter == null)
            {
                Debug.LogError("Failed to find Picked Character");
                return;
            }

            _handlerLoading.OpenLoadingScreen(true, HandlerLoading.Text.Saving);
            _handlerLoading.ActiveLoadingText(true);
            
            _handlerLoading.SetLoadingText("Create Character in Cloud");
            await SetCharacterData();

            _handlerLoading.SetLoadingText("Sending Character to Client");
            UpdateUserDataLocally();
            await UniTask.Delay(4000);
            
            _handlerLoading.SetLoadingText("Update User Payload");
            
            await _cacheUserInfo.payload.UpdatePayload();
            await _cacheUserInfo.inventory.Install(_cacheItemInfo);

            _handlerLoading.ActiveLoadingText(false);
        }

        private void UpdateUserDataLocally()
        {
            _cacheUserInfo.data.SetUnitSkin(_unit.Skin.currentMaterialName);
        }
        
        private async UniTask SetCharacterData()
        {
            var bundleItems = PickedCharacter.Bundle.BundledItems;
            
            var args = new 
            {   
                SkinValue = (string)_unit.Skin.currentMaterialName,
                PickedItems = PickedItemNames(bundleItems)
            };
            
            await PlayFabHandler.ExecuteCloudScript
                (PlayFabHandler.Function.SetCharacterToUser, args);
        }

        private static string PickedItemNames(List<string> bundledItems)
        {
            string pickedItemNames = null;
            foreach (var itemName in bundledItems)
            {
                pickedItemNames += itemName + ";";
            }

            return pickedItemNames;
        }
    }
}
