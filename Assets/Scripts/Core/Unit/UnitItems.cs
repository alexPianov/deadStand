using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Playstel
{
    public class UnitItems : MonoBehaviourPun
    {
        [Header("List")]
        public List<Item> inventoryItems = new ();
        public ObscuredInt _maxBagSlots;
        
        private Unit _unit;

        public void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        public void SetMaxBagSlots(int MaxBagSlots)
        {
            _maxBagSlots = MaxBagSlots;
        }
        
        public void AddItem(Item item)
        {
            if(item == null) return;
            inventoryItems.Add(item);
        }

        #region Get

        public int GetMaxBagSlots()
        {
            return _maxBagSlots;
        }

        public List<Item> GetItemList(ItemInfo.Catalog itemCatalog = ItemInfo.Catalog.Null, 
            ItemInfo.Class itemClass = ItemInfo.Class.Null, 
            ItemInfo.Subclass itemSubclass = ItemInfo.Subclass.Null)
        {
            return ItemHandler.Search(inventoryItems, itemCatalog, itemClass, itemSubclass);
        }

        public List<Item> GetSameItems(string itemName)
        {
            if (string.IsNullOrEmpty(itemName)) return null;
            return inventoryItems.FindAll(item => item.info.itemName == itemName);
        }
        
        public List<Item> GetItems(List<string> itemNames)
        {
            var items = new List<Item>();
            
            if (itemNames == null) return null;

            foreach (var itemName in itemNames.Distinct().ToList())
            {
                items.AddRange
                    (inventoryItems.FindAll(item => item.info.itemName == itemName));
            }

            return items;
        }
        
        public Item IsAlreadyExists(ItemInfo newItemInfo)
        {
            if (GetItemList() == null || GetItemList().Count == 0) return null;
            
            Debug.Log("New Item: " + newItemInfo.itemName);
            
            return GetItemList().Find(item => item.info.itemName == newItemInfo.itemName);
        }

        #endregion

        #region Remove

        public bool HaveSameItem(Item newItem)
        {
            var items = GetItemList(newItem.info.itemCatalog, newItem.info.itemClass);
            
            var result = items.Find(item => item.info.itemName == newItem.info.itemName);
            
            if (newItem.info.itemCatalog == ItemInfo.Catalog.Weapons)
            {
                return result;
            }
            
            if (newItem.info.itemCatalog == ItemInfo.Catalog.Support)
            {
                return result;
            }
            
            if (PhotonNetwork.InRoom)
            {
                if (newItem.info.itemCatalog == ItemInfo.Catalog.Character)
                {
                    return result;
                }
            }
            
            return false;
        }
        
        public void RemoveSameClassItem(Item newItem)
        {
            var items = GetItemList(newItem.info.itemCatalog, newItem.info.itemClass);

            foreach (var item in items)
            {
                RemoveItem(item, false);
            }
        }

        public void RemoveSameSubClassItem(Item newItem)
        {
            var items = GetItemList(newItem.info.itemCatalog, 
                newItem.info.itemClass, newItem.info.ItemSubclass);

            foreach (var item in items)
            {
                RemoveItem(item, false);
            }
        }

        public void RemoveInventoryItems()
        {
            var stuffItems = GetItemList(ItemInfo.Catalog.Character, ItemInfo.Class.Stuff);
            RemoveItems(stuffItems);
            
            var rigItems = GetItemList(ItemInfo.Catalog.Character, ItemInfo.Class.Rig);
            RemoveItems(rigItems);
        }

        public void RemoveItems(ItemInfo.Catalog itemsCatalog)
        {
            var itemsToRemove = GetItemList(itemsCatalog);
            RemoveItems(itemsToRemove);
        }

        public void RemoveItems(List<Item> items)
        {
            foreach (var item in items)
            {
                RemoveItem(item);
            }
        }
        
        public void RemoveItem(Item item, bool destroyScripableObject = true)
        {
            if (inventoryItems.Contains(item))
            {
                inventoryItems.Remove(item);
                _unit.HandleItems.Remove(item);
                
                item.DestroyInstance();
                item.DestroyItem();
                
                //if(destroyScripableObject) Destroy(item);
            }
        }

        public void RemoveBeardBlockingItems()
        {
            var items = GetItemList(ItemInfo.Catalog.Character, ItemInfo.Class.Stuff);
            
            var blockingItems = items
                .FindAll(item => item.info.GetBlockSubclass() == ItemInfo.Subclass.Beard).Count;

            if (blockingItems > 0)
            {
                var blockingItem = items
                    .Find(item => item.info.ItemSubclass == ItemInfo.Subclass.Beard);

                Debug.Log("Remove blocking item");
                
                RemoveItem(blockingItem, false);
            }
        }

        #endregion
    }
}
