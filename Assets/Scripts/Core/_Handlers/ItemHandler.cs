using Photon.Pun;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    public static class ItemHandler
    {
        public static ItemInfo CreateItemInfo(CatalogItem catalogItem)
        {
            var itemInfo = ScriptableObject.CreateInstance<ItemInfo>();
            itemInfo.SetCatalogData(catalogItem);
            return itemInfo;
        }

        public static Item CreateItem(ItemInfo itemInfo)
        {
            var item = ScriptableObject.CreateInstance<Item>();
            item.SetInfo(itemInfo);
            return item;
        }

        public static ItemImpact CreateImpact(ItemInfo itemInfo, int hostId = 0)
        {
            var item = ScriptableObject.CreateInstance<ItemImpact>();
            item.SetInfo(itemInfo);
            item.SetHostId(hostId);
            return item;
        }
        
        public static List<Item> Search(List<Item> searchField, 
            ItemInfo.Catalog itemCatalog = ItemInfo.Catalog.Null, 
            ItemInfo.Class itemClass = ItemInfo.Class.Null, 
            ItemInfo.Subclass itemSubclass = ItemInfo.Subclass.Null)
        {
            if (searchField == null)
                return null;
            
            if (itemCatalog == ItemInfo.Catalog.Null) 
                return searchField; 
            
            var catalogItems = searchField
                .FindAll(item => item.info.itemCatalog == itemCatalog);
            
            if (itemClass == ItemInfo.Class.Null) return catalogItems; 

            var classItems = catalogItems
                .FindAll(item => item.info.itemClass == itemClass);

            if (itemSubclass == ItemInfo.Subclass.Null) return classItems; 
            
            var subclassItems = classItems
                .FindAll(item => item.info.ItemSubclass == itemSubclass);

            return subclassItems;
        }
        
        public static List<ItemInfo> Search(List<ItemInfo> searchField,
            ItemInfo.Catalog itemCatalog = ItemInfo.Catalog.Null, 
            ItemInfo.Class itemClass = ItemInfo.Class.Null,
            ItemInfo.Subclass itemSubclass = ItemInfo.Subclass.Null)
        {
            if (itemCatalog == ItemInfo.Catalog.Null) 
                return searchField; 
            
            var catalogItems = searchField
                .FindAll(item => item.itemCatalog == itemCatalog);
            
            if (itemClass == ItemInfo.Class.Null) return catalogItems; 

            var classItems = catalogItems
                .FindAll(item => item.itemClass == itemClass);

            if (itemSubclass == ItemInfo.Subclass.Null) return classItems; 
            
            var subclassItems = classItems
                .FindAll(item => item.ItemSubclass == itemSubclass);

            return subclassItems;
        }
    }
}
