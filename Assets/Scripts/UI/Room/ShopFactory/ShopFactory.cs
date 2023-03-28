using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class ShopFactory : UiFactory
    {
        [Header("Slot")] 
        public SlotName SlotName;

        [Header("Refs")] 
        public Transform focus;
        public TextMeshProUGUI shopName;

        private void Start()
        {
            GetTraderItems();
        }

        private void GetTraderItems()
        {
            var shopItemsString = GetUnit().Interaction.DialogNpc.npcCharacter
                .GetCharacterData(NpcCharacter.DataKey.ItemsForTrade);

            if (string.IsNullOrEmpty(shopItemsString))
            {
                Debug.LogError("Failed to find items for trade");
                return;
            }

            var shopItems = DataHandler.Deserialize(shopItemsString);

            shopItems.TryGetValue("ItemCatalog", out var itemCatalogString);
            var itemCatalog = KeyStore.GetCatalog(itemCatalogString);

            shopItems.TryGetValue("ItemClass", out var itemClassString);
            var itemClass = KeyStore.GetClass(itemClassString);

            shopItems.TryGetValue("ItemSubclass", out var itemSubClassString);
            var itemSubClass = KeyStore.GetSubclass(itemSubClassString);

            CreateShopSlots(itemCatalog, itemClass, itemSubClass);

            shopName.text = itemClass + " Store";
        }

        private int slotCount;
        public async UniTask CreateShopSlots(
            ItemInfo.Catalog itemCatalog = ItemInfo.Catalog.Null, 
            ItemInfo.Class itemClass = ItemInfo.Class.Null,
            ItemInfo.Subclass itemSubclass = ItemInfo.Subclass.Null)
        {
            var items = GetItemsFromSource(ItemSource.External,
                itemCatalog, itemClass, itemSubclass);
                
            var orderedItems = items.OrderBy(item => item.info.GetItemPrice()).ToList();
                
            for (int i = 0; i < orderedItems.Count; i++)
            {
                var slotObject = await CreateSlot(SlotName);
                var itemInfo = orderedItems[i].info;

                if (slotObject.TryGetComponent(out ShopSupportSlot support))
                {
                    Debug.Log("ShopSupportSlot: " + itemInfo.itemName);
                    support.SetItemInfo(itemInfo);
                    
                    if (itemInfo.itemClass == ItemInfo.Class.Ammo)
                    {
                        ActiveAvailableAmmoButton(itemInfo, GetUnit(), slotObject);
                    }
                }
                
                if (slotObject.TryGetComponent(out ShopWeaponSlot weaponSlot))
                {
                    weaponSlot.SetItemInfo(itemInfo, this);
                    
                    if (TryGetComponent(out ShopWeaponInterface weaponInterface))
                    {
                        weaponSlot.SetInterface(weaponInterface);
                    }
                }
            }
        }

        private void ActiveAvailableAmmoButton(ItemInfo itemInfo, Unit unit, GameObject slot)
        {
            var currentFirearm = unit.HandleItems.GetFirearm();

            var ammoName = currentFirearm.Ammo.GetAmmoInfo().itemName;

            slot.SetActive(ammoName == itemInfo.itemName);
        }
    }
}