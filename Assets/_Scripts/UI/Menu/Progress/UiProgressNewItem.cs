using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiProgressNewItem : MonoBehaviour
    {
        public UiProgressItemShow ItemShow;
        public Button takeButton;
        private GameObject _spawnedPrefab;
        public List<Item> _loadedItems = new();

        [Inject] private CacheUserInfo _cacheUserInfo;
        [Inject] private CacheItemInfo _cacheItemInfo;
        [Inject] private CacheAudio _cacheAudio;

        private void Start()
        {
            takeButton.onClick.AddListener(OpenNextLoadedItem);
        }
        
        public void LoadNewItems(List<ItemInstance> itemInstances)
        {
            List<string> newItemNames = new();
            _loadedItems = new();

            foreach (var itemInstance in itemInstances)
            {
                newItemNames.Add(itemInstance.DisplayName);
            }

            var items = _cacheItemInfo
                .CreateItemList(newItemNames.ToArray(), ItemInfo.Catalog.Character);

            var passValue = _cacheUserInfo.data
                .GetUserData(UserData.UserDataType.BattlePass);

            var battlePass = bool.Parse(passValue);

            if (battlePass)
            {
                _loadedItems = items;
            }
            else
            {
                foreach (var item in items)
                {
                    if (!item.info.IsPremium())
                    {
                        _loadedItems.Add(item);
                    }
                }
            }
            
            OpenNextLoadedItem();
        }

        private void OpenNextLoadedItem()
        {
            if (_loadedItems.Count > 0)
            {
                _cacheAudio.Play(CacheAudio.MenuSound.OnGetItem);
                ItemShow.OpenItem(_loadedItems[0]);
                _loadedItems.Remove(_loadedItems[0]);
            }
            else
            {
                ItemShow.ItemPanel(false);
                _loadedItems.Clear();
            }
        }
    }
}