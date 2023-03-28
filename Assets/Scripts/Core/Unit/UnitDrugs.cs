using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Playstel
{
    public class UnitDrugs : MonoBehaviour
    {
        public Item _item;
        public Unit _unit;
        
        public ObscuredInt healthDose;
        public ObscuredFloat speedDose;
        public ObscuredInt drugDuration;
        
        public enum DrugImpact
        {
            Health, Speed, Duration
        }
        
        private void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        public bool OnDragEffect()
        {
            return _item;
        }

        public void Use(Item item)
        {
            Debug.Log("Set " + item.info.itemName);
            
            _item = item;

            healthDose = GetValueInt(DrugImpact.Health);
            speedDose = GetValueFloat(DrugImpact.Speed);
            drugDuration = GetValueInt(DrugImpact.Duration);
            
            if (drugDuration > 0)
            {
                activeDuration = true;
            }
            else
            {
                ActiveDrug();
                ClearDrug();
            }

            //RemoveDrug(item);
        }

        public bool speedMode;
        public bool activeDuration;
        public float durationTime;
        public float iterationTime;
        
        private void Update()
        {
            if (activeDuration)
            {
                iterationTime += Time.deltaTime;

                if (iterationTime > 1)
                {
                    iterationTime = 0;
                    ActiveDrug();
                }
                
                durationTime += Time.deltaTime;
                
                if (durationTime > drugDuration)
                {
                    durationTime = 0;
                    activeDuration = false;
                    
                    iterationTime = 0;
                    speedMode = false;
                    
                    ClearDrug();
                }
            }
        }

        private void ActiveDrug()
        {
            if (speedDose > 0)
            {
                speedMode = true;
            }

            if (healthDose > 0)
            {
                if (_unit.photonView.IsMine)
                {
                    _unit.Buffer.ChangeHealth(healthDose, true);
                }
            }

            CreateEffect();
        }

        private void ClearDrug()
        {
            healthDose = 0;
            speedDose = 0;
            drugDuration = 0;

            RemoveDrug(_item);
        }

        private async void RemoveDrug(Item item)
        {
            var items = new List<Item> { item };
            
            var request = HandlerHostRequest
                .GetRemoveRequest(items, ItemInfo.Catalog.Support);
            
            await _unit.HostOperator
                .Run(UnitHostOperator.Operation.Remove, request);
        }

        public float GetSpeedBonus()
        {
            if (!speedMode) return 1;
            if (speedDose == 0) return 1; 
            return speedDose;
        }

        private int GetValueInt(DrugImpact drugImpact)
        {
            var value = _item.info
                .GetTypedData(ItemInfo.DataType.StatInt, drugImpact.ToString());

            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }

            return int.Parse(value);
        }

        private float GetValueFloat(DrugImpact drugImpact)
        {
            var value = _item.info
                .GetTypedData(ItemInfo.DataType.StatFloat, drugImpact.ToString());

            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }

            return float.Parse(value) / 100;
        }
        
        private void CreateEffect()
        {
            if (healthDose > 0)
            {
                _unit.VFX.Create(UnitVFX.Visual.Hea, true);
            }
            
            if (speedDose > 0)
            {
                _unit.VFX.Create(UnitVFX.Visual.Spd, true);
            }
        }
    }
}