using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    [RequireComponent(typeof(Button), typeof(UIElementLoadByType))]
    public class UiCharacterQuickSave : MonoBehaviour
    {
        [Inject] private Unit _unit;
        [Inject] private HandlerLoading _handlerLoading;

        private UIElementLoadByType _elementLoad;
        private Button _button;

        private void Awake()
        {
            _elementLoad = GetComponent<UIElementLoadByType>();
            _button = GetComponent<Button>();
            _button.onClick.AddListener(SaveCharacterItems);
        }

        public async void SaveCharacterItems()
        {
            UpdateSkinNameLocally();
            
            var inventoryCharacterItems = _unit.Items
                .GetItemList(ItemInfo.Catalog.Character);
            
            var args = new 
            {   
                SkinValue = (string)_unit.Skin.currentMaterialName,
                PickedItems = PickedItemNames(inventoryCharacterItems)
            };
            
            _handlerLoading.OpenLoadingPopup(true, HandlerLoading.Text.Saving);
            
            await PlayFabHandler.ExecuteCloudScript
                (PlayFabHandler.Function.SetCharacterToUser, args);
                
            await _unit.CacheUserInfo.payload.UpdatePayload();
            
            _handlerLoading.OpenLoadingPopup(false);

            _elementLoad.Load();
        }

        private static string PickedItemNames(List<Item> items)
        {
            string pickedItemNames = null;
            foreach (var item in items)
            {
                pickedItemNames += item.info.itemName + ";";
            }

            return pickedItemNames;
        }
        
        private void UpdateSkinNameLocally()
        {
            _unit.CacheUserInfo.data
                .SetUnitSkin(_unit.Skin.currentMaterialName);
        }
    }
}
