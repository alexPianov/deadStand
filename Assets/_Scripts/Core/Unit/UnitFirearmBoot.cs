using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Playstel
{
    public class UnitFirearmBoot : MonoBehaviour
    {
        [Inject] private LocationInstaller _locationInstaller;
        public AudioSource WeaponAudio;
        private Unit _unit;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        public async UniTask Boot(Item _item, GameObject firearmInstance)
        {
            if (firearmInstance.TryGetComponent(out ItemFirearm firearm))
            {
                firearm.SetComponents(_unit, _item.info);
                firearm.SetLaserSight();
                
                var firearmAmmo = await GetAmmo(firearmInstance);
                _unit.Ammo.SetCurrentAmmo(firearmAmmo);
                firearm.SetAmmo(firearmAmmo);
                
                var reloader = await CreateReloader(firearmInstance);
                firearm.SetReloader(reloader);

                var sfx = await CreateItemSFX(firearmInstance);
                
                sfx.SetItemFX(_item.info);
                sfx.SetAudioSource(WeaponAudio);
                
                var effects = await CreateEffects(firearmInstance);
                firearm.SetEffects(effects, sfx);
                
                _unit.Buffer.ChangeHolders(firearmAmmo.holders, true);
                _unit.Buffer.ChangeBullets(firearm.Stat.maxBullets);
            }
        }

        private async UniTask<ItemAmmo> GetAmmo(GameObject firearmInstance)
        {
            if (firearmInstance.TryGetComponent(out ItemStat itemStat))
            {
                return await _unit.Ammo.Get(itemStat.ammoName);
            }
            return null;
        }
        
        private async UniTask<ItemFirearmReloader> CreateReloader(GameObject firearmInstance)
        {
            return await _locationInstaller.InstantiateComponent
                <ItemFirearmReloader>(firearmInstance);
        }
        
        private async UniTask<ItemFirearmEffects> CreateEffects(GameObject firearmInstance)
        {
            return await _locationInstaller.InstantiateComponent
                <ItemFirearmEffects>(firearmInstance);
        }
        
        private async UniTask<ItemSFX> CreateItemSFX(GameObject firearmInstance)
        {
            return await _locationInstaller.InstantiateComponent
                <ItemSFX>(firearmInstance);
        }
    }
}