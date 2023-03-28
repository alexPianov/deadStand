using System;
using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    public class UnitItemsUse : MonoBehaviour
    {
        // private Unit _unit;
        //
        // private void Awake()
        // {
        //     _unit = GetComponent<Unit>();
        // }
        //
        // public void Use(List<Item> items)
        // {
        //     foreach (var item in items)
        //     {
        //         Use(item);
        //     }
        // }
        //
        // public void Use(Item item)
        // {
        //     if(item.info.itemClass == ItemInfo.Class.Grenade)
        //     {
        //         CreateGrenade(item);
        //     }
        //
        //     if(item.info.itemClass == ItemInfo.Class.Drug)
        //     {
        //         UseDrug(item);
        //     }
        // }
        //
        // private async void CreateGrenade(Item item)
        // {
        //     if (_unit.Items.GetItemList().Contains(item))
        //     {
        //         if (_unit.HandleItems.currentItem != item)
        //         {
        //             await _unit.ItemSpawner.SpawnItem(item);
        //             _unit.HandleItems.PickItem(item);
        //         }
        //     }
        // }
        //
        // private void UseDrug(Item item)
        // {
        //     if (_unit.Items.GetItemList().Contains(item))
        //     {
        //         _unit.Drugs.Use(item);
        //     }
        // }

    }
}