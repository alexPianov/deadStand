using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiFactory : MonoBehaviour
    {
        [Header("Container")]
        public Transform slotContainer;

        public enum SlotName
        {
            ItemSlot, ItemSlotTrigger, ShopWeaponSlot, ShopSupportSlot, 
            UserSlotMenu, ProgressBundleSlot, 
            CustomizeUnitSlot, CustomizeWeaponSlot, CustomizeCarSlot,
            CharacterSlot, UserSlotRoom, UserSlotRoomMine
        }

        public enum ItemSource
        {
            Unit, User, External
        }

        [Inject] private Unit _unit;
        [Inject] private LocationInstaller _locationInstaller;
        [Inject] private CacheUserInfo _cacheUserInfo;
        [Inject] private CacheItemInfo _cacheItemInfo;

        public List<Item> GetItemsFromSource(
            ItemSource source,
            ItemInfo.Catalog itemCatalog = ItemInfo.Catalog.Null, 
            ItemInfo.Class itemClass = ItemInfo.Class.Null, 
            ItemInfo.Subclass itemSubclass = ItemInfo.Subclass.Null)
        {
            var itemList = new List<Item>();
            
            if (source == ItemSource.Unit)
            {
                itemList = _unit.Items.GetItemList(itemCatalog, itemClass, itemSubclass);
            }
            
            if (source == ItemSource.User)
            {
                itemList = _cacheUserInfo.inventory.GetItemList(itemCatalog, itemClass, itemSubclass);
            }

            if (source == ItemSource.External)
            {
                itemList = _cacheItemInfo.CreateItemList(itemCatalog, itemClass, itemSubclass);
            }

            return itemList;
        }

        public async UniTask<GameObject> CreateSlot(SlotName slotName)
        {
            var slot = await _locationInstaller
                .LoadElement<GameObject>(slotName.ToString(), slotContainer);

            slot.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            
            return slot;
        }

        public void ClearFactoryContainer()
        {
            for (int i = 0; i < slotContainer.childCount; i++)
            {
                Destroy(slotContainer.GetChild(i).gameObject);
            }
        }

        public Unit GetUnit()
        {
            return _unit;
        }

        public CacheUserInfo GetUserInfo()
        {
            return _cacheUserInfo;
        }
        
        public CacheItemInfo GetItemInfo()
        {
            return _cacheItemInfo;
        }
    }
}
