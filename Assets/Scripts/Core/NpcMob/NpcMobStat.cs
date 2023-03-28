using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class NpcMobStat : MonoBehaviour
    {
        [Header("Stats")]
        public ObscuredInt health;
        public ObscuredFloat regeneration;
        public ObscuredFloat damage = 12f;
        public ObscuredFloat impactChance = 50;
        public ObscuredFloat level;
        public ObscuredString impact;
        public ObscuredString loot;
        public ObscuredFloat lootChance;
        public ObscuredFloat updateTargetTime;

        [Header("Control Stats")]
        public ObscuredFloat runSpeed;
        public ObscuredFloat viewRange;
        public ObscuredFloat attackRange;
        public ObscuredFloat respawnTime;

        public ObscuredBool isDead;
        public ObscuredBool canMove;

        public delegate void MaxEnduranceUpdate(float maxEndurance);
        public event MaxEnduranceUpdate OnMaxEnduranceUpdate;

        public delegate void MaxHealthUpdate(float maxHealth);
        public event MaxHealthUpdate OnMaxHealthUpdate;

        [Inject]
        private CacheItemInfo _cacheItemInfo;

        public void SetStat(string mobName)
        {
            var unitSetup = _cacheItemInfo
                .GetItemInfo(mobName, ItemInfo.Catalog.Setup, ItemInfo.Class.Unit);
            
            SetStatInt(unitSetup);
            SetStatString(unitSetup);

            //GetComponent<UnitInfo>().SetUnitName(mobName);
        }

        private void SetStatString(ItemInfo unitSetup)
        {
            var customDataString = unitSetup.GetTypedData(ItemInfo.DataType.StatString);

            impact = GetStatString(customDataString, "Impact");
            loot = GetStatString(customDataString, "Loot");
        }

        private void SetStatInt(ItemInfo unitSetup)
        {
            var customDataInt = unitSetup.GetTypedData(ItemInfo.DataType.StatInt);

            health = GetStat(customDataInt, "Health");
            regeneration = GetStat(customDataInt, "Regeneration");
            damage = GetStat(customDataInt, "Damage");
            impactChance = GetStat(customDataInt, "ImpactChance");
            level = GetStat(customDataInt, "Level");
            runSpeed = GetStat(customDataInt, "RunSpeed");
            viewRange = GetStat(customDataInt, "ViewRange");
            attackRange = GetStat(customDataInt, "AttackRange");
            lootChance = GetStat(customDataInt, "LootChance");
            updateTargetTime = GetStat(customDataInt, "TargetTime");
            respawnTime = GetStat(customDataInt, "RespawnTime");
        }

        private int GetStat(Dictionary<string, string> customData, string statName)
        {
            return DataHandler.GetUnsafeValueInt(customData, statName);
        }

        private string GetStatString(Dictionary<string, string> customData, string statName)
        {
            return DataHandler.GetUnsafeValue(customData, statName);
        }
    }
}
