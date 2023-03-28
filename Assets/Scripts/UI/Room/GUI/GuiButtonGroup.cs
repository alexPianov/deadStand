using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Playstel.UI
{
    public class GuiButtonGroup : MonoBehaviour
    {
        public List<GuiHandleItem> buttonList = new();

        public ItemInfo.Catalog itemCatalog;
        public SlotName itemSlotName;
        public Transform focus;
        
        public enum SlotName
        {
            WeaponSlot, SupportSlot
        }
        
        [Inject]
        private LocationInstaller _locationInstaller;

        [Inject]
        private Unit _unit;
        
        private async void Start()
        {
            await UniTask.WaitUntil(() => _unit.Boot.IsFinish);

            RefreshButtons();
        }

        private void RefreshButtons()
        {
            var items = _unit.Items.GetItemList(itemCatalog);

            foreach (var item in items)
            {
                AddHandleItemSlot(item);
            }
        }

        public async void AddHandleItemSlot(Item item)
        {
            var slot = await CreateSlot(itemSlotName.ToString());

            if(!slot) 
            {
                Debug.LogError("Failed to create handle item slot " + item.info.itemName); return;
            }

            var handleItem = slot.GetComponent<GuiHandleItem>();
            
            if(!handleItem) 
            {
                Debug.LogError("Failed to get GuiHandleItem component in item slot " + item.info.itemName); return;
            }
            
            handleItem.SetItemInfo(item);
            buttonList.Add(handleItem);
        }

        public void RemoveHandleItemSlot(Item item)
        {
            var removingSlot = buttonList.Find(info => info == item);

            _unit.Items.RemoveItem(item);
            Destroy(removingSlot);
            buttonList.Remove(removingSlot);
        }
        
        private async UniTask<GameObject> CreateSlot(string slotName)
        {
            var slot = await _locationInstaller.LoadElement<GameObject>(slotName, transform);

            slot.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            
            return slot;
        }

        public void MoveFocus(Transform slot)
        {
            if (focus)
            {
                focus.SetParent(slot);
                focus.SetSiblingIndex(1);
                focus.localPosition = new Vector3(0, 0, 0);
            }
        }
    }
}
