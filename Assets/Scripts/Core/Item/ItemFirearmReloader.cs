using CodeStage.AntiCheat.ObscuredTypes;
using Photon.Pun;
using Cysharp.Threading.Tasks;
using EventBusSystem;
using UnityEngine;

namespace Playstel
{
    public class ItemFirearmReloader : MonoBehaviourPun
    {
        private ItemFirearm _firearm;

        public void SetFirearm(ItemFirearm firearm)
        {
            _firearm = firearm;
        }

        public async UniTask Reload()
        {
            ResetAttackSlider();
            
            await ReloadAnimation();
            
            _firearm.Unit.Buffer.ChangeHolders();

            var maxBullets = _firearm.Stat.maxBullets;
            _firearm.Unit.Buffer.ChangeBullets(maxBullets);
        }

        public async UniTask ReloadEachShot()
        {
            ResetAttackSlider();
            
            await ReloadAnimation();
        }

        public async UniTask ReloadAnimation()
        {
            _firearm.Unit.Animation.ItemAnimation(UnitAnimation.Actions.Reload, _firearm.Info);
            
            var reloadTime = _firearm.Stat.reloadTime;
            
            ReloadBarAnimation(reloadTime);
            await _firearm.Unit.Animation.AwaitAnimation(reloadTime);
        }

        public void ResetAttackSlider()
        {
            if(!_firearm.Unit.photonView.IsMine) return;
            if(_firearm.Unit.IsNPC) return;
            
            EventBus.RaiseEvent<IAttackHandler>(h => h.HandleAttackSliderValue(0));
        }

        private void ReloadBarAnimation(float reloadTime)
        {
            if(!_firearm.Unit.photonView.IsMine) return;
            if(_firearm.Unit.IsNPC) return;

            EventBus.RaiseEvent<IAmmoHandler>(h => h.HandleHolderReload
                (reloadTime));
        }

        #region Magazine

        Quaternion startRot = new (0, 0, 0, 1);
        Vector3 startPos = new (0, 0, 0);
        
        Quaternion holderRot = new (0, 0, 0, 1);
        Vector3 holderPos = new (0, 0, 0);
        public void UnloadMagazine()
        {
            if(!_firearm.holderInstance) return;
            
            holderRot = _firearm.holderInstance.localRotation;
            holderPos = _firearm.holderInstance.localPosition;
            
            var holderInstance = _firearm.holderInstance;

            holderInstance.SetParent(_firearm.Unit.Root.leftHand);
            
            holderInstance.localRotation = startRot;
            holderInstance.localPosition = startPos;
        }

        public void LoadMagazine()
        {
            if(!_firearm.holderInstance) return;
            
            var holderInstance = _firearm.holderInstance; 
            
            holderInstance.SetParent(transform);
            holderInstance.localRotation = holderRot;
            holderInstance.localPosition = holderPos;
        }

        #endregion
    }
}
