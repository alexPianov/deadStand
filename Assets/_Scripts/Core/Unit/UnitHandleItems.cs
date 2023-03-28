using CodeStage.AntiCheat.ObscuredTypes;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    [RequireComponent(typeof(UnitAnimation))]
    public class UnitHandleItems : MonoBehaviourPun
    {
        [HideInInspector]
        public List<Item> handleItems = new ();
        public Item currentItem;
        public ItemController currentItemController;
        public ItemStat currentItemStat;
        private ItemFirearm _itemFirearm;
        private Unit _unit;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        #region Listener

        public void PickItem(Item newItem, bool instantPick = false)
        {
            if (!newItem)
            {
                Debug.LogError("Failed to pick item: New item is empty"); return;
            }
            
            if (!newItem.info)
            {
                Debug.LogError("Failed to pick item: New item info is empty"); return;
            }
            
            if (currentItem && currentItemController && currentItemStat)
            {
                if(currentItem.info)
                {
                    if (newItem.info.itemName == currentItem.info.itemName)
                    {
                        return;
                    }
                }
            }
            
            if (_unit.Await && _unit.Await.IsAwaiting()) return;
            
            currentItem = newItem;

            SetCurrentItemComponents();

            _unit.Animation.ItemAnimation(UnitAnimation.Actions.Pick, currentItem.info, instantPick);
            _unit.Animation.AwaitAnimation();
        }
        
        private void SetCurrentItemComponents()
        {
            if (currentItem.instance == null)
            {
                Debug.Log("Warning! Failed to find instance of item: " + currentItem.info.itemName);
            }
            
            currentItemStat = currentItem.instance.GetComponent<ItemStat>();
            currentItemController = currentItem.instance.GetComponent<ItemController>();
        }

        public void DropHandleItems(ItemInfo.Catalog catalog)
        {
            var droppedItems = _unit.Items.GetItemList(catalog);

            foreach (var item in droppedItems)
            {
                item.DropInstance();
            }
        }

        #endregion

        #region Handler

        public void Add(Item item)
        {
            handleItems.Add(item);
            
            if (item.info.itemClass == ItemInfo.Class.Firearm)
            {
                _itemFirearm = item.instance.GetComponent<ItemFirearm>();
            }
        }

        public void Remove(Item item)
        {
            if (handleItems.Contains(item)) 
                handleItems.Remove(item);
        }
        
        public ItemFirearm GetFirearm()
        {
            return _itemFirearm;
        }

        public List<GameObject> GetHandleInstanceList()
        {
            var items = new List<GameObject>();
            
            foreach (var item in handleItems)
            {
                items.Add(item.instance);
            }

            return items;
        }

        #endregion
    }
}
