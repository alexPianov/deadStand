using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    public class UnitAnimationReceiver : MonoBehaviour
    {
        public UnitHandleItems unitHandleItems;
        public UnitCallback unitCallback;
        public Unit Unit;

        #region Reloading
        
        public void DetachMagazine()
        {
            if(!unitHandleItems.currentItem || !unitHandleItems.currentItem.info) return;
            if(unitHandleItems.currentItem.info.itemClass == ItemInfo.Class.Firearm)
            {
                Unit.HandleItems.GetFirearm().Reloader.UnloadMagazine();
                
                FirearmReloadSound(CacheSoundClips.ReloadType.Unload);
            }
        }

        public void AttachMagazine()
        {
            if(!unitHandleItems.currentItem || !unitHandleItems.currentItem.info) return;
            
            if(unitHandleItems.currentItem.info.itemClass == ItemInfo.Class.Firearm)
            {
                Unit.HandleItems.GetFirearm().Reloader.LoadMagazine();
                SoundAttachMagazine();
            }
        }

        public void SoundAttachMagazine()
        {
            FirearmReloadSound(CacheSoundClips.ReloadType.Load);
        }


        public void SoundRevolverCock()
        {
            FirearmReloadSound(CacheSoundClips.ReloadType.RevolverCock, true);
        }

        public void SoundShotgunAmmo()
        {
            FirearmReloadSound(CacheSoundClips.ReloadType.FillCartridge, true);
        }

        public void SoundFirearmCock()
        {
            FirearmReloadSound(CacheSoundClips.ReloadType.Cock);
        }
        
        public void SoundRevolverShells()
        {
            FirearmReloadSound(CacheSoundClips.ReloadType.RevolverShells, true);
        }
        
        private void FirearmReloadSound(CacheSoundClips.ReloadType sound, bool ignoreSubclass = false)
        {
            if(!unitHandleItems.currentItem || !unitHandleItems.currentItem.info) return;
            
            if(unitHandleItems.currentItem.info.itemClass == ItemInfo.Class.Firearm)
            {
                Unit.HandleItems.GetFirearm().Effects.PlayReloadSound(sound, ignoreSubclass);
            }
        }

        #endregion

        public void ThrowGrenade()
        {
            if(unitHandleItems.currentItem.info.itemClass == ItemInfo.Class.Grenade)
            {
                unitCallback.Throw();
            }
        }

        public void PickLastItem()
        {
            
        }

        public void PickSound()
        {

        }

        public void MeleeSwingSound()
        {

        }

        public void MeleeCollisionDamage()
        {
            if (unitHandleItems.currentItem.info.itemClass == ItemInfo.Class.Melee)
            {
                unitHandleItems.currentItemController.itemMelee.Damage(true);
            }
        }

        public void MeleeCollisionDamageStop()
        {
            if (unitHandleItems.currentItem.info.itemClass == ItemInfo.Class.Melee)
            {
                unitHandleItems.currentItemController.itemMelee.Damage(false);
            }
        }

        private const float laserWidthNormal = 0.05f;
        private const float laserWidthSmall = 0.008f;
        public void EnableLaserAim()
        {
            if(!unitHandleItems.currentItemController.itemFirearm.LaserSight) return;
            unitHandleItems.currentItemController.itemFirearm.LaserSight.EditWidth(laserWidthNormal);
        }

        public void DisableLaserAim()
        {
            if(!unitHandleItems.currentItemController.itemFirearm.LaserSight) return;
            unitHandleItems.currentItemController.itemFirearm.LaserSight.EditWidth(laserWidthSmall);
        }
    }
}
