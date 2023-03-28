using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using EventBusSystem;
using Photon.Pun;
using UnityEngine;

namespace Playstel
{
    public class UnitCallback : MonoBehaviourPun
    {
        private Unit _unit;

        public void Start()
        {
            _unit = GetComponent<Unit>();
        }

        #region Firearm Shot

        public void FirearmShot()
        {
            if(!photonView.IsMine) return;
            
            var aimTarget = _unit.Aim.GetAimTarget().position;
            var firearm = _unit.HandleItems.GetFirearm();

            List<Vector3> orientations = new List<Vector3>();

            for (int i = 0; i < firearm.Stat.bulletsPerShot; i++)
            {
                orientations.Add(_unit.HandleItems.GetFirearm().GetRandomOrientation(aimTarget));
            }
            
            photonView.RPC(nameof(RPC_FirearmShot), RpcTarget.All, orientations.ToArray() as object);
        }

        [PunRPC]
        private void RPC_FirearmShot(object shotOrientations)
        {
            Vector3[] orientations = (Vector3[]) shotOrientations;
            _unit.HandleItems.GetFirearm().Shot(orientations);
        }

        #endregion
        
        #region Firearm Reload

        public void FirearmReload()
        {
            if (!photonView.IsMine) return;
            if (_unit.Await.IsAwaiting()) return;

            var firearm = _unit.HandleItems.GetFirearm();

            if (firearm.Ammo.Payload.GetBullets() == firearm.Stat.maxBullets)
            {
                Debug.Log("No need to reload");
                return;
            }
            
            int firearmHolders = firearm.Ammo.Payload.GetHolders();

            if (_unit.IsNPC)
            {
                firearmHolders = 10;
            }
            
            if (firearmHolders < 1)
            {
                Debug.Log("No holders for reloading");
                firearm.Effects.PlayReloadSound(CacheSoundClips.ReloadType.DryFire, true);
                return;
            }
            
            photonView.RPC(nameof(RPC_FirearmReload), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_FirearmReload()
        {
            _unit.HandleItems.GetFirearm().Reloader.Reload();
        }

        #endregion

        #region Use (From Master)
        
        public void UseItems(string[] items)
        {
            if(items == null || items.Length == 0) return;
            
            photonView.RPC(nameof(RPC_UseItems), RpcTarget.All, items as object);
        }
        
        [PunRPC]
        private void RPC_UseItems(object items)
        {
            var itemsList = (string[]) items;
            
            var itemsToUse = _unit.Items.GetItems(itemsList.ToList());
            
            //_unit.ItemsUse.Use(itemsToUse);
        }

        #endregion

        #region Death

        [ContextMenu("Death Test")]
        public void DeathTest()
        {
            Death(0, transform.position);
        }

        private ObscuredInt _respawnDelay = 1900;
        public async void Death(int killerViewId, Vector3 hitPosition)
        {
            if(!photonView.IsMine) return;
            
            _unit.Aim.Aiming(false);

            if (!_unit.IsNPC) _unit.Camera.GetCameraObserver().FollowKiller(killerViewId);

            photonView.RPC(nameof(RPC_Death), RpcTarget.All, hitPosition);

            photonView.RPC(nameof(RPC_Announce), RpcTarget.All, killerViewId, 
                _unit.photonView.ViewID, GetExperience());

            await UniTask.Delay(_respawnDelay);

            var request = HandlerHostRequest.GetRespawnRequest(killerViewId);
            _unit.HostOperator.Run(UnitHostOperator.Operation.Respawn, request);
        }

        [PunRPC]
        private void RPC_Death(Vector3 hitPosition)
        {
            _unit.Death.Death(hitPosition);

            if (_unit.photonView.IsMine && !_unit.IsNPC)
            {
                var deaths = StatHandler.GetDeaths(_unit.photonView.Owner);
                var newDeathsValue = deaths + 1;
                PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Deaths, newDeathsValue);
            }
        }

        private ObscuredInt experienceCoeff = 2;
        private int GetExperience()
        {
            int userLevel = _unit.Experience.currentLevel;
            if (userLevel == 0) userLevel = 1;
            return 50 + userLevel * experienceCoeff;
        }

        [PunRPC]
        private void RPC_Announce(int killerId, int victimId, int victimExp)
        {
            var victim = PhotonView.Find(victimId);
            var victimUnit = victim.gameObject.GetComponent<Unit>();
            
            if (killerId == 0)
            {
                FragAnnounce($"<color=#{victimUnit.Fraction.UnitColor()}> {victimUnit.unitName}</color>" +
                             $" is killed by himself");
                return;
            }

            var killer = PhotonView.Find(killerId);
            var killerUnit = killer.gameObject.GetComponent<Unit>();

            if (killer == null)
            {
                FragAnnounce($"<color=#{victimUnit.Fraction.UnitColor()}> {victimUnit.unitName}</color>" +
                          $" is killed by himself");
                return;
            }
            
            FragAnnounce($"<color=#{killerUnit.Fraction.UnitColor()}> " +
                         $"{killerUnit.unitName}</color> killed " +
                         $"{victimUnit.unitName}</color>");

            if (victimUnit.IsNPC) victimUnit.GetComponent<UnitAiStatistics>().AddDeath();
            if (killerUnit.IsNPC) killerUnit.GetComponent<UnitAiStatistics>().AddFrag(); 

            if (killerUnit.photonView.IsMine && !killerUnit.IsNPC)
            {
                if(killerUnit.name == victimUnit.name) return;
                
                killerUnit.Buffer.ChangeExperience(victimExp);
                killerUnit.Buffer.ChangeFrags();
            }
        }

        int num = 0;
        
        [ContextMenu("Announce Test")]
        public void AnnounceTest()
        {
            num++;
            FragAnnounce("Test No. " + num);
        }

        private void FragAnnounce(string text)
        {
            EventBus.RaiseEvent<IAnnounceHandler>(h => h.HandleValue(text));
        }
        
        #endregion

        #region Game Over (From Master)

        public void GameOver()
        {
            photonView.RPC(nameof(RPC_GameOver), RpcTarget.All);
        }
        
        [PunRPC]
        private void RPC_GameOver()
        {
            if (photonView.IsMine)
            {
                _unit.DeathScreen.OpenGameOverScreen();
            }
        }

        #endregion
        
        #region Respawn (From Master)

        public void Respawn(int killerId)
        {
            photonView.RPC(nameof(RPC_Respawn), RpcTarget.All, killerId);
        }
        
        [PunRPC]
        private void RPC_Respawn(int killerId)
        {
            _unit.Death.Respawn();
        }

        #endregion
        
        #region Impact 
        
        public void ReceiveImpact(string impactName, int hostId, bool fromField = false)
        {
            if(!photonView.IsMine) return;
            
            photonView.RPC(nameof(RPC_ReceiveImpact), RpcTarget.All, 
                impactName, hostId, fromField);
        }
        
        [PunRPC]
        private void RPC_ReceiveImpact(string impactName, int hostId, bool fromField)
        {
            _unit.Impact.AddImpact(impactName, hostId, fromField);
        }

        #endregion

        #region Melee

        public void Melee()
        {
            if (!photonView.IsMine) return;

            photonView.RPC(nameof(RPC_CurrentItemMelee), RpcTarget.All);
        }

        [PunRPC]
        public void RPC_CurrentItemMelee()
        {
            _unit.HandleItems.currentItemController.itemMelee.Use();
        }

        #endregion

        #region Throw

        public void ThrowSwing()
        {
            if (!photonView.IsMine) return;

            photonView.RPC(nameof(RPC_ThrowSwing), RpcTarget.All);
        }

        [PunRPC]
        public void RPC_ThrowSwing()
        {
            _unit.HandleItems.currentItemController.GetGrenade().ThrowSwing();
        }

        public void Throw()
        {
            if (!photonView.IsMine) return;

            _unit.GrenadeThrow.ThrowingLine(false);

            var grenade = _unit.HandleItems.currentItemController.GetGrenade();

            grenade.transform.LookAt(_unit.Aim.GetAimTarget());
            var pos = grenade.transform.position;
            var rot = grenade.transform.rotation;

            var throwPower = _unit.GrenadeThrow.GetThrowPower();

            photonView.RPC(nameof(RPC_CurrentItemThrow), RpcTarget.All, throwPower, pos, rot);
        }

        [PunRPC]
        public void RPC_CurrentItemThrow(float throwPower, Vector3 pos, Quaternion rot)
        {
            _unit.HandleItems.currentItemController.GetGrenade().Throw(throwPower, pos, rot);
        }

        #endregion

    }
}
