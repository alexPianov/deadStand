using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace Playstel
{
    [CreateAssetMenu(menuName = "User/Inventory")]
    public class UserInventory : ScriptableObject
    {
        [Header("Setup")]
        public UserPayload userPayload;
        public bool includeDupes;

        public List<Item> userInventory = new ();
        public List<Item> grantedItems = new();

        private bool _isLoaded;

        public List<Item> GetItemList(ItemInfo.Catalog itemCatalog = ItemInfo.Catalog.Null, 
            ItemInfo.Class itemClass = ItemInfo.Class.Null, 
            ItemInfo.Subclass itemSubclass = ItemInfo.Subclass.Null)
        {
            return ItemHandler.Search(userInventory, itemCatalog, itemClass, itemSubclass);
        }

        public async UniTask Install(CacheItemInfo cacheItemInfo)
        {
            userInventory.Clear();

            var inventory = userPayload.GetPlayFabPayload().UserInventory;
            
            if (inventory == null)
            {
                Debug.LogError("Inventory is null");
                return;
            }

            userInventory = CreateItemList(cacheItemInfo, inventory);

            CheckGrantedItemsForNull();
        }

        private void CheckGrantedItemsForNull()
        {
            for (int i = 0; i < grantedItems.Count; i++)
            {
                if (grantedItems[i] == null)
                {
                    grantedItems.Remove(grantedItems[i]);
                }
            }
        }

        private List<Item> CreateItemList(CacheItemInfo cacheItemInfo, List<ItemInstance> inventory)
        {
            var itemInventory = new List<Item>();

            itemInventory
                .AddRange(CreateCatalogItemList(cacheItemInfo, inventory, 
                    ItemInfo.Catalog.Character));
            
            itemInventory
                .AddRange(CreateCatalogItemList(cacheItemInfo, inventory, 
                    ItemInfo.Catalog.Shop));

            return itemInventory;
        }

        private List<Item> CreateCatalogItemList(CacheItemInfo _cacheItemInfo, 
            List<ItemInstance> inventory, ItemInfo.Catalog catalog)
        {
            var catalogItems = inventory
                .FindAll(item => item.CatalogVersion == catalog.ToString());

            List<string> catalogItemNamesList = new List<string>();

            foreach (var item in catalogItems)
            {
                if (includeDupes)
                {
                    catalogItemNamesList.Add(item.ItemId);
                }
                else
                {
                    if(!catalogItemNamesList.Contains(item.ItemId))
                        catalogItemNamesList.Add(item.ItemId);
                }
            }

            var characterItemList = _cacheItemInfo
                .CreateItemList(catalogItemNamesList.ToArray(), ItemInfo.Catalog.Character);

            CheckBattlePass(characterItemList);

            return characterItemList;
        }

        private void CheckBattlePass(List<Item> characterItemList)
        {
            var battlePass = userPayload.userData
                .GetUserData(UserData.UserDataType.BattlePass);

            if (string.IsNullOrEmpty(battlePass))
            {
                return;
            }

            if (!bool.Parse(battlePass))
            {
                var premiumItems = characterItemList.FindAll(item => item.info.IsPremium());

                foreach (var premiumItem in premiumItems)
                {
                    characterItemList.Remove(premiumItem);
                }
            }
        }

        public void SetGrantedItem(Item item)
        {
            Debug.Log("SetGrantedItem: " + item.info.itemName);
            grantedItems.Add(item);
        }

        public List<Item> GetGrantedItems()
        {
            return grantedItems;
        }

        public void ExcludeFormGrantedItems(ItemInfo itemInfo)
        {
            for (int i = 0; i < grantedItems.Count; i++)
            {
                var grantedItem = grantedItems[i];
                
                if (grantedItem.info.itemName == itemInfo.itemName)
                {
                    grantedItems.Remove(grantedItem);
                }
            }
        }
    }
}
