using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Playstel
{
    public class CacheItemInfo : MonoBehaviour
    {
        public InstallItemInfo install;
        
        public ItemInfo GetItemInfo(
            string itemName, 
            ItemInfo.Catalog itemCatalog = ItemInfo.Catalog.Null, 
            ItemInfo.Class itemClass = ItemInfo.Class.Null,
            ItemInfo.Subclass itemSubclass = ItemInfo.Subclass.Null)
        {
            var list = GetItemInfoList(itemCatalog, itemClass, itemSubclass);

            var result = list.Find(info => info.itemName == itemName);

            if (result == null)
            {
                Debug.Log("Failed to find " + itemName + " in cache");
                return null;
            }

            return result;
        }

        public List<ItemInfo> GetItemInfoList(
            ItemInfo.Catalog itemCatalog = ItemInfo.Catalog.Null, 
            ItemInfo.Class itemClass = ItemInfo.Class.Null,
            ItemInfo.Subclass itemSubclass = ItemInfo.Subclass.Null)
        {
            return ItemHandler.Search(install.GetItemInfoList(), itemCatalog, itemClass, itemSubclass);
        }

        public List<Item> CreateItemList(ItemInfo.Catalog itemCatalog = ItemInfo.Catalog.Null, 
            ItemInfo.Class itemClass = ItemInfo.Class.Null,
            ItemInfo.Subclass itemSubclass = ItemInfo.Subclass.Null)
        {
            var itemInfoList = GetItemInfoList(itemCatalog, itemClass, itemSubclass);
            
            var itemList = new List<Item>();
            
            foreach (var itemInfo in itemInfoList)
            {
                itemList.Add(CreateItemByInfo(itemInfo));
            }

            return itemList;
        }

        public List<Item> CreateItemList(string[] itemNames, 
            ItemInfo.Catalog catalog = ItemInfo.Catalog.Null,
            ItemInfo.Class itemClass = ItemInfo.Class.Null, 
            ItemInfo.Subclass itemSubclass = ItemInfo.Subclass.Null)
        {
            var createdItems = new List<Item>();

            if (itemNames.Length == 0) return new List<Item>();
            
            var catalogItems = GetItemInfoList(catalog, itemClass, itemSubclass);

            for (int i = 0; i < itemNames.Length; i++)
            {
                var itemInfo = catalogItems.Find(item => item.itemName == itemNames[i]);
                createdItems.Add(ItemHandler.CreateItem(itemInfo));
            }

            return createdItems;
        }
        
        public Item CreateItemByName(string itemName, ItemInfo.Catalog itemCatalog = ItemInfo.Catalog.Null, 
            ItemInfo.Class itemClass = ItemInfo.Class.Null,
            ItemInfo.Subclass itemSubclass = ItemInfo.Subclass.Null)
        {
            var itemInfoList = GetItemInfoList(itemCatalog, itemClass, itemSubclass);

            var itemInfo = itemInfoList.Find(item => item.itemName == itemName);

            return CreateItemByInfo(itemInfo);
        }
        
        private Item CreateItemByInfo(ItemInfo itemInfo)
        {
            return ItemHandler.CreateItem(itemInfo);
        }
    }
}
