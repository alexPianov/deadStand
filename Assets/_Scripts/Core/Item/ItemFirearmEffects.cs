using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class ItemFirearmEffects : MonoBehaviour
    {
        private ItemFirearm _firearm;
        private ParticleSystem _sparkEffect;
        private ParticleSystem _shellEffect;
        private ItemSFX _itemSfx;
        
        [HideInInspector]
        [Inject] public CacheSoundClips CacheSoundClips;

        public void SetFirearm(ItemFirearm firearm)
        {
            _firearm = firearm;
        }

        public void SetSFX(ItemSFX itemSfx)
        {
            _itemSfx = itemSfx;
        }

        public async void CreateShotEffects()
        {
            _itemSfx = GetComponent<ItemSFX>();
            _sparkEffect = await CreateEffect("V_Spk", _firearm.sparkPos);
            _shellEffect = await CreateEffect("V_Shl", _firearm.shellPos);
        }
        
        private float _shakeFactor = 0.35f;
        public void ShotEffect()
        {
            AudioEffect();

            var unit = _firearm.Unit;
            
            unit.Animation.ItemAnimation(UnitAnimation.Actions.Shot, _firearm.Info);
            unit.Animation.ItemAnimation(UnitAnimation.Actions.Recoil, _firearm.Info);
            
            if (_sparkEffect) _sparkEffect.Play();
            if (_shellEffect) _shellEffect.Play();

            if (unit.photonView.IsMine)
            {
                if (unit.IsNPC) return;

                unit.Camera.GetCameraObserver().Shake(_shakeFactor, 0.2f);
            }
        }

        private void AudioEffect()
        {
            if (!_itemSfx) return;
            
            // if (_firearm.Ammo.GetAmmoInfo().itemName == "Fuel")
            // {
            //     if(_itemSfx.GetAudioSource().isPlaying) return;
            // }
            
            _itemSfx.PlaySound(ItemSFX.Sounds.Fir);
        }

        public void ClearCurrentClip()
        {
            _itemSfx.GetAudioSource().Stop();
        }
        
        private async UniTask<ParticleSystem> CreateEffect(string effectName, Transform container)
        {
            var customData = _firearm.Ammo.GetAmmoInfo().GetTypedData(ItemInfo.DataType.FX);
            if (customData == null) { Debug.LogError("Failed to find ammo FX"); return null; }
            var sparkName = DataHandler.GetUnsafeValue(customData, effectName);
            if(string.IsNullOrEmpty(sparkName)) return null;
            var ammoSpark = await AddressablesHandler.Get(sparkName, transform);
            if(!ammoSpark) return null;
            ammoSpark.transform.rotation = container.rotation;
            ammoSpark.transform.position = container.position;
            return ammoSpark.GetComponent<ParticleSystem>();
        }
        
        public void PlayReloadSound(CacheSoundClips.ReloadType reloadType, bool ignoreSubclass = false)
        {
            var subclass = _firearm.Info.ItemSubclass;

            if (ignoreSubclass) subclass = ItemInfo.Subclass.Null;
            
            var soundClip = CacheSoundClips.GetReloadSound(reloadType, subclass);
            
            if (!soundClip) return;
            
            _itemSfx.PlayClip(soundClip, 0.5f);
        }
    }
}