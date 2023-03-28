using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using EventBusSystem;
using Photon.Pun;
using UnityEngine;

namespace Playstel
{
    public class ItemController : MonoBehaviourPun
    {
        public ItemStat itemStat;
        public ItemFirearm itemFirearm;
        [HideInInspector] public ItemMelee itemMelee;
        [HideInInspector] public ItemGrenade itemGrenade;
        
        public bool singleMode;
        
        public Item _item;
        public Unit _unit;

        private const float _singleThreshold = 0.2f;
        public void Initialize(Item item, Unit unit)
        {
            _item = item;
            _unit = unit;

            if (GetComponent<ItemStat>()) itemStat = GetComponent<ItemStat>();
            
            if (GetComponent<ItemFirearm>())
            {
                itemFirearm = GetComponent<ItemFirearm>();
                singleMode = itemStat.attackRate > _singleThreshold;
            }
            
            if (GetComponent<ItemMelee>()) itemMelee = GetComponent<ItemMelee>();
            if (GetComponent<ItemGrenade>()) itemGrenade = GetComponent<ItemGrenade>();
        }

        private float time = 0;
        private ObscuredFloat minAttackRate = 0.08f;
        private bool blockAttack;

        public void Update()
        {
            if (!_item) return;

            if (!_unit || !_unit.photonView) { Destroy(gameObject); return; }

            if (!_unit.photonView.IsMine || _unit.IsNPC) return;

            if (_unit.Await.IsAwaiting()) return;

            Attack();
        }

        public void Attack()
        {
            if (!_unit.HandleItems.currentItem || !_unit.HandleItems.currentItem.info) return;
            if (_unit.HandleItems.currentItem.info.itemName != _item.info.itemName) return;

            if (itemFirearm && itemFirearm.Ammo)
            {
                if (itemFirearm.Ammo.Payload.GetBullets() < 1) return;
                
                if (singleMode) { SingleMode(); return; }

                if (_unit.Aim.attack) HitsChain();
            }
        }

        private bool completionSound;
        private void CompletionSound()
        {
            if (_unit.Aim.attack)
            {
                completionSound = false;
            }
            else
            {
                if (!completionSound)
                {
                    Debug.Log("CompletionSound");
                    completionSound = true;
                    
                    itemFirearm.Effects.ClearCurrentClip();
                    itemFirearm.Effects
                        .PlayReloadSound(CacheSoundClips.ReloadType.CompletionSound, true);
                }
            }
        }

        private void SingleMode()
        {
            if (time <= itemStat.attackRate)
            {
                time += Time.deltaTime;
                EditAttackSlider();
            } 

            if (_unit.Aim && _unit.Aim.attack)
            {
                if (time >= itemStat.attackRate)
                {
                    SingleHit();
                } 
            }
        }

        private async void SingleHit()
        {
            _unit.Callback.FirearmShot();

            if (itemFirearm.Stat.eachShotReload) return;
            
            time = 0;
            
            if (_unit.IsNPC) return;
            
            await UniTask.Delay(200);
            DisableUIAttackModeUi();
        }
        
        private void EditAttackSlider()
        {
            if(!_unit.photonView.IsMine) return;
            if(_unit.IsNPC) return;
            
            EventBus.RaiseEvent<IAttackHandler>(h =>
                h.HandleAttackSliderValue(time));
        }

        private void DisableUIAttackModeUi()
        {
            if(!_unit.photonView.IsMine) return;
            if(_unit.IsNPC) return;

            EventBus.RaiseEvent<IAttackHandler>(h => h.HandleAttackMode(false));
        }

        private void HitsChain()
        {
            time += Time.deltaTime;

            if (time >= itemStat.attackRate)
            {
                time = 0;

                if (itemStat.attackRate < minAttackRate)
                {
                    Debug.LogError("Shoot rate span is too short"); return;
                }
                
                if (itemFirearm)
                {
                    _unit.Callback.FirearmShot();
                }
            }
        }
        
        #region Melee

        public void MeleeAttack()
        {
            Debug.Log("MeleeAttack");
            
            if (itemStat.GetItemInfo().ItemSubclass == ItemInfo.Subclass.Sawing)
            {
                if (itemStat.attackRate < minAttackRate)
                {
                    Debug.LogError("Shoot rate span is too short");
                    return;
                }

                time += Time.deltaTime;

                if (time >= itemStat.attackRate)
                {
                    time = 0;
                    _unit.Callback.Melee();
                }
            }
            else
            {
                _unit.Callback.Melee();
                _unit.Aim.Attack(false);
            }
        }

        public void MeleeUse()
        {
            itemMelee.Use();
        }

        #endregion

        #region Throw

        public void ThrowAttack()
        {
            Debug.Log("ThrowAttack");
            _unit.Callback.ThrowSwing();
            _unit.Aim.Attack(false);
        }

        public ItemGrenade GetGrenade()
        {
            if (itemGrenade) return itemGrenade;
            else return null;
        }

        #endregion
    }
}
