using CodeStage.AntiCheat.ObscuredTypes;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class ItemAmmo : ObjectPool
    {
        public ItemAmmoPayload Payload;

        [Header("Stat")] 
        public ObscuredInt speed;
        public ObscuredString material;
        public ObscuredInt poolSize;
        public ObscuredInt holders;

        private ItemInfo _itemInfo;
        
        public GameObject GetProjectile(Transform muzzle = null)
        {
            return GetFromPool(_itemInfo.itemName, muzzle);
        }

        public ItemInfo GetAmmoInfo()
        {
            return _itemInfo;
        }

        public void SetAmmoPayload(ItemAmmoPayload payload)
        {
            Payload = payload;
        }
        
        public async UniTask SetAmmoData(ItemInfo itemInfo)
        {
            if (!itemInfo) return;

            _itemInfo = itemInfo;

            SetStatInt();
            SetMaterial();

            await CreateProjectilePool();
        }

        private void SetMaterial()
        {
            var customDataMat = _itemInfo.GetUnsafeValue(ItemInfo.DataType.Material.ToString());

            if (customDataMat == null)
            {
                Debug.LogError("Failed to find ammo customData"); return;
            }
            
            material = customDataMat;
        }

        private void SetStatInt()
        {
            var customDataInt = _itemInfo.GetTypedData(ItemInfo.DataType.StatInt);

            if (customDataInt == null)
            {
                Debug.LogError("Failed to find ammo customData"); return;
            }

            speed = DataHandler.GetUnsafeValueInt(customDataInt, "BulletSpeed");
            poolSize = DataHandler.GetUnsafeValueInt(customDataInt, "PoolSize");
            holders = DataHandler.GetUnsafeValueInt(customDataInt, "Holders");
        }

        private async Task CreateProjectilePool()
        {
            await CreatePool(_itemInfo.itemName, poolSize, material, transform);
        }
    }
}
