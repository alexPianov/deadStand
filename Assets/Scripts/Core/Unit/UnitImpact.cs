using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Playstel
{
    public class UnitImpact : MonoBehaviourPun
    {
        [Header("Unit")] 
        public AudioSource audioSource;
        public List<ItemImpact> currentImpacts = new ();
        
        private ObscuredFloat lowestSpeedPenalty;
        private ObscuredFloat defaultSpeedPenalty = 1;
        
        private Unit _unit;

        [Inject] private CacheItemInfo _cacheItemInfo;

        public enum Impact
        {
            Fire, Shock, Toxic
        }

        private void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        private void Start()
        {
            lowestSpeedPenalty = defaultSpeedPenalty;
        }

        [ContextMenu("Shock")]
        public void AddShock()
        {
            AddImpact("Shock", photonView.ViewID);
        }
        
        [ContextMenu("Fire")]
        public void AddFire()
        {
            AddImpact("Fire", photonView.ViewID);
        }

        [ContextMenu("Toxic")]
        public void AddToxic()
        {
            AddImpact("Toxic", photonView.ViewID);
        }

        public void AddImpact(string impactName, int hostId, bool fromField = false)
        {
            Debug.Log("Add Impact: " + impactName);

            if (GetUnitImpact(impactName))
            {
                if (fromField)
                {
                    SetDamage(GetUnitImpact(impactName).additionDamage, hostId);
                }
                
                return;
            }
            
            var impact = CreateImpact(impactName);
            currentImpacts.Add(impact);
            
            SetEffects(impactName);
            UpdateSpeedPenalty();
        }

        private ItemImpact GetUnitImpact(string impactName)
        {
            return currentImpacts.Find(impact => impact.itemInfo.itemName == impactName);
        }

        private ItemImpact CreateImpact(string impactName)
        {
            var impactsInfo = _cacheItemInfo
                .GetItemInfo(impactName, ItemInfo.Catalog.Setup, ItemInfo.Class.Impact);

            return ItemHandler.CreateImpact(impactsInfo, photonView.ViewID);
        }
        
        private void UpdateSpeedPenalty()
        {
            if (currentImpacts.Count == 0)
            {
                lowestSpeedPenalty = defaultSpeedPenalty;
            }
            
            foreach (var impact in currentImpacts)
            {
                if (impact.speedPenalty < lowestSpeedPenalty)
                {
                    lowestSpeedPenalty = impact.speedPenalty;
                }
            }
        }

        private void Update()
        {
            if (currentImpacts.Count > 0)
            {
                for (int i = 0; i < currentImpacts.Count; i++)
                {
                    if (currentImpacts[i] == null)
                    {
                        currentImpacts.Remove(currentImpacts[i]);
                        continue;
                    }
                    
                    ImpactThread(currentImpacts[i]);
                }
            }

            UpdateSpeedPenalty();
        }

        private float impactDurationTime;
        private float impactDamageRateTime;
        private void ImpactThread(ItemImpact impact)
        {
            impactDurationTime += Time.deltaTime;
            impactDamageRateTime += Time.deltaTime;

            if (impactDamageRateTime > impact.impactDamageRate)
            {
                impactDamageRateTime = 0;
                SetDamage(impact.impactDamage, impact.hostId);
            }

            if (impactDurationTime > impact.impactDuration)
            {
                impactDurationTime = 0;
                currentImpacts.Remove(impact);
            }
        }
        
        private void SetEffects(string impact)
        {
            if (impact == Impact.Fire.ToString())
            {
                _unit.VFX.Create(UnitVFX.Visual.Brn, true);
                
                ImpactSound(KeyStore.SFX_IMPACT_FIRE);
            }

            if (impact == Impact.Toxic.ToString())
            {
                _unit.VFX.Create(UnitVFX.Visual.Psn, true);
            }

            if (impact == Impact.Shock.ToString())
            {
                _unit.VFX.Create(UnitVFX.Visual.Shk, true);
                
                ImpactSound(KeyStore.SFX_IMPACT_SHOCK);
            }
        }

        private async UniTask ImpactSound(string effectName)
        {
            var collisionSound = await AddressablesHandler.Load<AudioClip>(effectName);

            audioSource.clip = collisionSound;
            audioSource.Play();
        }

        private void SetDamage(int damage, int hostId)
        {
            _unit.Damage.SetDamage(damage, hostId, transform.position);
        }

        public void CancelImpacts()
        {
            _unit.VFX.StopAllEffects();
            currentImpacts.Clear();
        }

        public float GetSpeedPenalty()
        {
            return lowestSpeedPenalty;
        }
    }
}
