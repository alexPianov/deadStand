using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Playstel
{
    [CreateAssetMenu(menuName = "Install/ItemInfo")]
    public class InstallItemInfo : ScriptableObject
    {
        private string[] catalogs =
        {
            "Setup", "Character", "Weapons", "Support", "Backpack", "Shop"
        };

        private List<ItemInfo> _itemsInfoList = new ();
        private CacheSprites _cacheSprites;

        public List<ItemInfo> GetItemInfoList()
        {
            return _itemsInfoList;
        }

        public async UniTask Install(CacheSprites cacheSprites)
        {
            _cacheSprites = cacheSprites;
            _itemsInfoList.Clear();
            
            LoadCounter.NewCounter();

            foreach (var catalog in catalogs)
            {
                GetCatalogJSON(catalog);
            }

            await UniTask.WaitUntil(() => LoadCounter.isLoaded);
        }

        private void GetCatalogJSON(string catalog)
        {
            GetCatalogItemsRequest request = new GetCatalogItemsRequest { CatalogVersion = catalog };
            PlayFabClientAPI.GetCatalogItems(request, OnGetItemsData, OnPlayFabError);
        }

        private void OnGetItemsData(GetCatalogItemsResult catalogResult)
        {
            foreach (var catalogItem in catalogResult.Catalog)
            {
                CreateItemInfo(catalogItem);
            }

            LoadCounter.AddIteration(catalogs.Length);
        }

        private void CreateItemInfo(CatalogItem catalogItem)
        {
            if (catalogItem == null)
            {
                Debug.LogError("Catalog Item is null"); return;
            }

            var itemInfo = ItemHandler.CreateItemInfo(catalogItem);
            _itemsInfoList.Add(itemInfo);

            SetItemSpriteToItemInfo(itemInfo);
        }

        private void SetItemSpriteToItemInfo(ItemInfo itemInfo)
        {
            if (!_cacheSprites) { Debug.LogError("Failed to find _cacheSprites"); return; }

            var sprite = _cacheSprites
                .GetSpriteFromAtlas(itemInfo.itemName, itemInfo.itemCatalog);

            if (sprite == null)
            {
                return;
            } 

            itemInfo.SetSpriteItem(sprite);
        }

        private void OnPlayFabError(PlayFabError obj)
        {
            Debug.Log("PlayFab Error: " + obj.GenerateErrorReport());
        }
    }
}
