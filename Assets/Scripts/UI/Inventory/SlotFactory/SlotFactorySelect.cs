using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Playstel
{
    public class SlotFactorySelect : MonoBehaviour
    {
        public List<Slot> selectSlots = new();
        
        [Header("Refs")]
        public GameObject summaryPanel;
        public GameObject selectAllButton;
        public TextMeshProUGUI totalPriceText;
        
        private SlotFactorySelectHandle _slotFactorySelectHandle;
        private SlotFactory _slotFactory;

        private void Awake()
        {
            _slotFactorySelectHandle = GetComponent<SlotFactorySelectHandle>();
            _slotFactory = GetComponent<SlotFactory>();
            selectAllButton.SetActive(true);
        }

        public SlotFactorySelectHandle GetSelectHandle()
        {
            return _slotFactorySelectHandle;
        }

        public void ReceiveSlot(Slot slot)
        {
            if(selectSlots.Contains(slot))
            {
                DeselectSlot(slot);
            }
            else
            {
                SelectSlot(slot);
            }
        }

        public void SelectSlot(Slot slot)
        {
            slot.renderer.Outline(true);
            AddPrice(slot.currentItem.info);
            selectSlots.Add(slot);
            UpdateSummaryPanel();
        }

        public void DeselectSlot(Slot slot)
        {
            if(!selectSlots.Contains(slot)) return;
            
            slot.renderer.Outline(false);
            SubtractPrice(slot.currentItem.info);
            selectSlots.Remove(slot);
            UpdateSummaryPanel();
        }

        public void UpdateSummaryPanel()
        {
            var active = selectSlots.Count > 0;

            if (!active) totalPrice = 0;
            
            summaryPanel.SetActive(active);
            
            selectAllButton.SetActive(!active);
            
            // if (_slotFactory.GetFactorySlotList().Count == 0)
            // {
            //     selectAllButton.SetActive(false);
            // }

            totalPriceText.text = totalPrice.ToString();

            _slotFactorySelectHandle.UpdateHandleButton();
        }

        private int totalPrice;
        private void AddPrice(ItemInfo info)
        {
            totalPrice += info.GetItemPrice();
        }

        private void SubtractPrice(ItemInfo info)
        {
            totalPrice -= info.GetItemPrice();
        }

    }
}
