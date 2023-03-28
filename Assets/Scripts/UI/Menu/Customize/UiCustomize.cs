using Playstel;
using UnityEngine;
using Zenject;

public class UiCustomize : UiFactory
{
    public Transform customizeButtonFocus;
    public Transform slotFocus;
    public UiCustomizeNotifications Notifications;

    public async void CreateCustomizeSlots(SlotName slotName, ItemInfo.Catalog itemCatalog,
        ItemInfo.Class itemClass, ItemInfo.Subclass itemSubclass)
    {
        CancelFocus();
        ClearFactoryContainer();
        ShowCustomizeObject(slotName);
        
        Notifications.UpdateNotifications();

        var items = GetItemsFromSource(ItemSource.User, itemCatalog, itemClass, itemSubclass);

        foreach (var item in items)
        {
            var slotInstance = await CreateSlot(slotName);

            if (slotInstance.TryGetComponent(out UiCustomizeUnitSlot unitSlot))
            {
                unitSlot.SetFocus(slotFocus);
                unitSlot.SetItemInfo(item.info);
                unitSlot.MarkAsJustGranted(IsJustGranted(item));
            }
            
            if (slotInstance.TryGetComponent(out UiCustomizeWeaponSlot weaponSlot))
            {
                weaponSlot.BindInstance(null);
                weaponSlot.SetItem(item.info);
                weaponSlot.MarkAsJustGranted(IsJustGranted(item));
            }
        }
    }

    private bool IsJustGranted(Item item)
    {
        var grantedItems = GetUserInfo().inventory.GetGrantedItems();
        return grantedItems.Find(grantedItem => grantedItem.info.itemName == item.info.itemName);
    }

    private void ShowCustomizeObject(SlotName slotName)
    {
        GetUnit().Renderer.Active(slotName == SlotName.CustomizeUnitSlot);

        if (slotName == SlotName.CustomizeCarSlot)
        {
            Debug.Log("Spawn Car");
        }

        if (slotName == SlotName.CustomizeWeaponSlot)
        {
            Debug.Log("Spawn Weapon");
        }
    }

    private void CancelFocus()
    {
        slotFocus.transform.SetParent(null);
        slotFocus.transform.position = new Vector3(0, 1000, 0);
    }

    public bool FoldierNotEmpty(UiCustomizeButton customizeButton)
    {
        var items = GetItemsFromSource
        (ItemSource.User, customizeButton.currentCatalog, 
            customizeButton.currentClass, customizeButton.currentSubclass);

        if (items == null || items.Count == 0) return false;
        
        return true;
    }

    public bool MaleBeard(UiCustomizeButton customizeButton)
    {
        if (customizeButton.currentSubclass == ItemInfo.Subclass.Beard)
        {
            return GetUnit().Skin.isMale;
        }
        
        return true;
    }
}
