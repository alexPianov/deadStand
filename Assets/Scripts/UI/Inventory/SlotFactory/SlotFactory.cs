using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Playstel
{
    [RequireComponent(typeof(SlotFactorySelect))]
    public class SlotFactory : UiFactory
    {
        [Header("List")]
        public List<SlotDropTrigger> slotTriggers = new();

        public ItemSource ItemSource;
        
        private ObscuredInt _fieldSize;

        private SlotFactorySelect _factorySelect;

        private void Awake()
        {
            _factorySelect = GetComponent<SlotFactorySelect>();
        }

        #region Install

        public async UniTask CreateSlotTriggers(int size)
        {
            _fieldSize = size;
            
            for (var i = 0; i < _fieldSize; i++)
            {
                var slot = await CreateSlot(SlotName.ItemSlotTrigger);
                var slotTrigger = slot.GetComponent<SlotDropTrigger>();
                slotTrigger.SetFactory(this);
                slotTriggers.Add(slotTrigger);
            }
        }

        public async void CreateSlots(
            ItemInfo.Catalog itemCatalog = ItemInfo.Catalog.Null, 
            ItemInfo.Class itemClass = ItemInfo.Class.Null, 
            ItemInfo.Subclass itemSubclass = ItemInfo.Subclass.Null)
        {
            await CreateSlots(GetItemsFromSource(ItemSource, itemCatalog, itemClass, itemSubclass));
        }

        private int slotCount;
        public async Task CreateSlots(List<Item> items)
        {
            if (items.Count> slotTriggers.Count)
            {
                Debug.Log("Slot Factory | Triggers count is smaller than items count");
                return;
            }
            
            for (var i = 0; i < items.Count; i++)
            {
                var slotObject = await CreateSlot(SlotName.ItemSlot);

                if (slotObject.TryGetComponent(out Slot slot))
                {
                    slot.InitSlot(items[i]);
                    slot.SetSlotFactory(this);
                    slotTriggers[i].AttachSlot(slot); 
                }
                else
                {
                    Debug.LogError("Failed to find a slot component in the spawned object");
                    Destroy(slotObject);
                }
            }
        }

        #endregion

        #region Handler
        
        public List<SlotDropTrigger> GetEmptyTriggers()
        {
            return slotTriggers
                .FindAll(item => !item.CurrentSlot);
        }

        public List<Item> GetFactoryItemList()
        {
            var items = new List<Item>();
            
            foreach (var slot in GetFactorySlotList())
                items.Add(slot.currentItem);

            return items;
        }

        public List<Slot> GetFactorySlotList()
        {
            var slots = new List<Slot>();

            var triggers = slotTriggers.FindAll(item => item.CurrentSlot);
            
            foreach (var slotTrigger in triggers)
                slots.Add(slotTrigger.CurrentSlot);

            return slots;
        }
        
        public SlotFactorySelect GetFactorySelect()
        {
            return _factorySelect;
        }

        public SlotFactory GetTargetFactory()
        {
            return _factorySelect.GetSelectHandle().targetFactory;
        }

        #endregion
    }
}
