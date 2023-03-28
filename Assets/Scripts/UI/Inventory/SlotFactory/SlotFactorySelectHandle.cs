using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class SlotFactorySelectHandle : MonoBehaviour
    {
        public Button buttonAll;
        public Button button;
        public Mode buttonMode;
        public SlotFactory targetFactory;

        private SlotFactorySelect _slotFactorySelect;
        
        [Inject]
        private CacheAudio _cacheAudio;
        
        public enum Mode
        {
            Move, Remove, Sell 
        }
        
        [Inject] private Unit _unit;
        private SlotFactory _slotFactory;
        
        private void Awake()
        {
            _slotFactory = GetComponent<SlotFactory>();
            _slotFactorySelect = GetComponent<SlotFactorySelect>();
            
            InitializeButton();
        }
        
        private void InitializeButton()
        {
            if (buttonMode == Mode.Move)
            {
                button.onClick.AddListener(MoveSelectItemsLocally);
            }
            
            if (buttonMode == Mode.Remove)
            {
                button.onClick.AddListener(RemoveSelectItemsFromInventory);
            }
            
            if (buttonMode == Mode.Sell)
            {
                button.onClick.AddListener(SellSelectItemsFromInventory);
            }

            buttonAll.onClick.AddListener(SelectAll);
        }

        public void UpdateHandleButton()
        {
            if (_slotFactory.ItemSource == UiFactory.ItemSource.External)
            {
                button.interactable = !ItemsOverload();
            }
        }
        
        private bool ItemsOverload()
        {
            if (!targetFactory) return false;
                
            if(targetFactory.GetEmptyTriggers().Count < _slotFactorySelect.selectSlots.Count) 
                return true;
            
            return false;
        }
        
        public void MoveSelectItemsLocally()
        {
            if (!targetFactory)
            {
                Debug.LogError("Failed to find targer factory");
                return;
            }

            var emptyTriggers = targetFactory.GetEmptyTriggers();
            var selectSlots = _slotFactorySelect.selectSlots;
            
            if(emptyTriggers.Count < selectSlots.Count) 
                return;
            
            for (var i = 0; i < emptyTriggers.Count; i++)
            {
                if(i > selectSlots.Count - 1) break;
                
                var slotToMove = selectSlots[i];
                
                slotToMove.drag.ReleaseFromTrigger();
                emptyTriggers[i].AttachSlot(slotToMove);
            }
            
            for (int i = 0; i < selectSlots.Count; i++)
            {
                selectSlots[i].DeselectSlot();
            }
            
            _cacheAudio.Play(CacheAudio.MenuSound.OnSlotMove);
        }
        
        public void RemoveSelectItemsLocally()
        {
            var selectSlots = _slotFactorySelect.selectSlots;
            
            for (int i = 0; i < selectSlots.Count; i++)
            {
                var slot = selectSlots[i];
                slot.drag.ReleaseFromTrigger();
                slot.RemoveSlot();
            }
            
            selectSlots.Clear();
            _slotFactorySelect.UpdateSummaryPanel();
        }

        public async void RemoveSelectItemsFromInventory()
        {
            if(!_unit.photonView.IsMine) return;
            
            var items = new List<Item>();
            
            foreach (var slot in _slotFactorySelect.selectSlots)
            {
                if(slot.currentItem) items.Add(slot.currentItem);
            }
            
            var request = HandlerHostRequest
                .GetRemoveRequest(items, ItemInfo.Catalog.Backpack);

            var result = await _unit.HostOperator
                .Run(UnitHostOperator.Operation.Remove, request);
            
            if(result == UnitHostOperator.Status.Reject) return;
            
            _cacheAudio.Play(CacheAudio.MenuSound.OnSlotMove);

            RemoveSelectItemsLocally();
        }

        public async void SellSelectItemsFromInventory()
        {
            if(!_unit.photonView.IsMine) return;
            
            var items = new List<Item>();
            
            foreach (var slot in _slotFactorySelect.selectSlots)
            {
                if(slot.currentItem) items.Add(slot.currentItem);
            }
            
            var request = HandlerHostRequest
                .GetSellRequest(items, ItemInfo.Catalog.Backpack);

            var result = await _unit.HostOperator
                .Run(UnitHostOperator.Operation.Sell, request);
            
            if(result == UnitHostOperator.Status.Reject) return;
            
            _cacheAudio.Play(CacheAudio.MenuSound.OnLootDrop);

            RemoveSelectItemsLocally();
        }

        public void SelectAll()
        {
            var slots = _slotFactory.GetFactorySlotList();

            foreach (var slot in slots)
            {
                slot.SelectSlot();
            }
        }
    }
}
