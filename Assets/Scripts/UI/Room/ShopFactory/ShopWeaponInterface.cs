using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

namespace Playstel
{
    [RequireComponent(typeof(ItemRotate))]
    public class ShopWeaponInterface: ShopItemInterface
    {
        public Transform instanceContainer;
        public Canvas weaponCanvas;
        public Canvas shopCanvas;
        public Canvas backgroundCanvas;
        public Camera instanceCamera;
        public TextMeshProUGUI weaponName;

        private GameObject _spawnedPrefab;

        public void ActiveWeaponPanel(bool state)
        {
            shopCanvas.enabled = !state;
            backgroundCanvas.enabled = !state;
            weaponCanvas.enabled = state;
            instanceCamera.enabled = state;
            
            if (state)
            {
                CreateInstance(GetCurrentItemInfo());
            }
            else
            {
                Destroy(_spawnedPrefab);
            }
        }

        public void SetWeaponName(ItemInfo itemInfo)
        {
            weaponName.text = itemInfo.itemName;
        }

        private async void CreateInstance(ItemInfo itemInfo)
        {
            if(!instanceContainer) return;
            
            if (_spawnedPrefab)
            {
                Destroy(_spawnedPrefab);
            }
            
            _spawnedPrefab = await AddressablesHandler.Get(itemInfo.itemName, instanceContainer);
            if(!this) return;
            if(!gameObject) return;
            if(!_spawnedPrefab) return;
            if(!GetComponent<ItemRotate>()) return;
            await GetComponent<ItemRotate>().SetObject(_spawnedPrefab);
        }
    }
}