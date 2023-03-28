using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UnitAmmo : MonoBehaviourPun
    {
        public List<ItemAmmo> ammoList = new();
        public Transform ammoContainer;

        [Header("Current Ammo")] 
        public ItemAmmo currentAmmo;
        
        [Inject] private LocationInstaller _locationInstaller;
        [Inject] private CacheItemInfo _cacheItemInfo;

        private Unit _unit;
        private void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        public void SetCurrentAmmo(ItemAmmo itemAmmo)
        {
            currentAmmo = itemAmmo;
        }

        public ItemAmmo GetCurrentAmmo()
        {
            return currentAmmo;
        }

        public async void Add(Item itemAmmo)
        {
            var ammo = GetAmmo(itemAmmo.info.itemName);

            if (ammo == null)
            {
                await CreateAmmo(itemAmmo.info.itemName);
            }
            else
            {
                _unit.Buffer.ChangeHolders(ammo.holders, true);
            }
        }

        public async UniTask<ItemAmmo> Get(string ammoName)
        {
            var result = GetAmmo(ammoName);

            if (result == null)
            {
                result = await CreateAmmo(ammoName);
            }

            return result;
        }

        private ItemAmmo GetAmmo(string ammoName)
        {
            foreach (var ammoItem in ammoList)
            {
                if (ammoItem.GetAmmoInfo().itemName == ammoName)
                {
                    return ammoItem;
                }
            }

            return null;
        }

        private async UniTask<ItemAmmo> CreateAmmo(string ammoName)
        {
            var ammoObject = CreateAmmoObject(ammoName);
            var ammo = await CreateAmmoComponent(ammoObject);
            var ammoPayload = await CreateAmmoPayloadComponent(ammoObject);
            
            var ammoInfo = GetAmmoInfo(ammoName);

            ammo.SetAmmoData(ammoInfo);
            ammo.SetAmmoPayload(ammoPayload);
            ammo.Payload.SetUnit(_unit);
            
            ammoList.Add(ammo);
            
            return ammo;
        }

        private ItemInfo GetAmmoInfo(string ammoName)
        {
            return _cacheItemInfo.GetItemInfo
                (ammoName, ItemInfo.Catalog.Support, ItemInfo.Class.Ammo);
        }

        private GameObject CreateAmmoObject(string ammoName)
        {
            var ammoObject = new GameObject();
            ammoObject.name = ammoName + " Container";
            ammoObject.transform.SetParent(ammoContainer);
            return ammoObject;
        }

        private async UniTask<ItemAmmo> CreateAmmoComponent(GameObject ammoObject)
        {
            return await _locationInstaller.InstantiateComponent<ItemAmmo>(ammoObject);
        }
        
        private async UniTask<ItemAmmoPayload> CreateAmmoPayloadComponent(GameObject ammoObject)
        {
            return await _locationInstaller.InstantiateComponent<ItemAmmoPayload>(ammoObject);
        }
    }
}