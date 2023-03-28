using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Playstel
{
    public class UnitDeath : MonoBehaviour
    {
        private Unit _unit;
        private NpcMobReset _mob;
        private ObscuredBool isDead;
        private ObscuredString deadTag = "DeadBody";

        [Inject] private CacheAudio _cacheAudio;

        private void Start()
        {
            _unit = GetComponent<Unit>();
            _mob = GetComponent<NpcMobReset>();
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void Death(Vector3 hitPos)
        {
            if (isDead) return;
            isDead = true;

            _cacheAudio.Play(CacheAudio.MenuSound.OnDead);
            _cacheAudio.Play(CacheAudio.MenuSound.OnSplash);
            
            _unit.Impact.CancelImpacts(); 
            
            if (_unit.Move) _unit.Move.CanMove(false);
            
            if (_unit.IsNPC)
            {
                _unit.GetComponent<UnitAiTargetLocal>().ReceiveTargets(false);
                _unit.GetComponent<UnitAiCamera>().UnitInfoCanvas.Active(false);
                _unit.GetComponent<UnitAiMove>().Active(false);
            }
            
            _unit.Ragdoll.RagdollMode(true);
            _unit.Ragdoll.AddForce(hitPos);
            gameObject.layer = 1;
            
            if (_mob) { RagdollStateMob(); return; }
            
            if(_unit.Camera) _unit.Camera.UnitInfoCanvas.Active(false);
            //if(_unit.Tokens) _unit.Tokens.CreateToken();
            
            _unit.HandleItems.DropHandleItems(ItemInfo.Catalog.Weapons);
            
            SaveAndRemoveWeapons();
        }

        private string[] savedWeapons;
        private void SaveAndRemoveWeapons()
        {
            savedWeapons = UnitHostOperator.GetItems(_unit, ItemInfo.Catalog.Weapons);
            _unit.Buffer.ChangeItems(savedWeapons, false, ItemInfo.Catalog.Weapons);
        }

        private void RagdollStateMob()
        {
            _mob.DropLoot();
            _mob.ReturnMob();
        }

        public async void Respawn()
        {
            Debug.Log("Respawn");
            
            _unit.AreaBehaviour.SetAreaBehaviour(UnitAreaBehaviour.Area.Unsafe, null, false);
            _unit.VFX.CreateSplash();
            
            _unit.Renderer.Active(false);

            GetComponent<NavMeshAgent>().enabled = false;

            await UniTask.Delay(300);

            transform.position = FractionSpawner.Spawner
                .GetSpawn(_unit.Fraction.currentFraction);

            if(_unit.Camera) _unit.Camera.GetCameraObserver().SetMaxZoom(false);
            
            await UniTask.Delay(400);
            
            _unit.VFX.CreateSplash();
            
            _unit.Ragdoll.RagdollMode(false);
            
            _unit.Buffer.ChangeItems(savedWeapons, true, ItemInfo.Catalog.Weapons);
            
            var nameToLayer = _unit.Fraction.currentFraction.ToString();
            gameObject.layer = LayerMask.NameToLayer(nameToLayer);
            
            await UniTask.Delay(100);

            if (_unit.Move) _unit.Move.CanMove(true);
            
            if (_unit.Camera)
            {
                _unit.Camera.UnitInfoCanvas.Active(true);
                
                if (!_unit.IsNPC)
                {
                    _unit.Camera.GetCameraObserver().Follow(_unit.transform);
                }
            }
            
            _unit.Renderer.Active(true);
            
            GetComponent<NavMeshAgent>().enabled = true;
            
            if (_unit.IsNPC)
            {
                _unit.GetComponent<UnitAiTargetLocal>().ReceiveTargets(true);
                _unit.GetComponent<UnitAiCamera>().UnitInfoCanvas.Active(true);
                _unit.GetComponent<UnitAiMove>().Active(true);
                _unit.TargetGlobal.UpdateOrientation();
            }

            isDead = false;

            _unit.AreaBehaviour.SetAreaBehaviour(UnitAreaBehaviour.Area.Safe, null, false);
        }
    }
}