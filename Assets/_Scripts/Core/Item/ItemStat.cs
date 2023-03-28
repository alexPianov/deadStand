using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    public class ItemStat : MonoBehaviour
    {
        private ItemInfo itemInfo;

        public ObscuredInt maxBullets, damage, bulletsPerShot, accuracy;
        public ObscuredFloat viewRange, attackRate, reloadTime, weight;
        public ObscuredBool eachShotReload;

        public ObscuredString ammoName;
        public ObscuredString impactName;
        
        public enum StatKey
        {
            Damage, AttackRate, Holder, Accuracy, ReloadTime, Impact, 
            Health, Speed, Duration, Holders, Weight, ViewRange
        }

        public void SetInfo(ItemInfo info)
        {
            itemInfo = info;

            SetStatInt(info);
            SetStatString(info);
            SetStatFloat(info);
        }

        private void SetStatInt(ItemInfo info)
        {
            var customDataInt = itemInfo
                .GetTypedData(ItemInfo.DataType.StatInt);

            if (customDataInt == null)
            {
                Debug.LogWarning(info.itemName + " doesn't contains Stat key");
                return;
            }

            damage = GetStat(customDataInt, StatKey.Damage.ToString());
            maxBullets = GetStat(customDataInt, StatKey.Holder.ToString());
            accuracy = GetStat(customDataInt, StatKey.Accuracy.ToString());

            viewRange = GetStat(customDataInt, "ViewRange");
            bulletsPerShot = GetStat(customDataInt, "BulletsPerShot");
        }

        private void SetStatString(ItemInfo info)
        {
            var customDataString = itemInfo
                .GetTypedData(ItemInfo.DataType.StatString);

            if (customDataString == null)
            {
                Debug.LogWarning(info.itemName + " doesn't contains Stat key");
                return;
            }

            ammoName = GetStatString(customDataString, "Ammo");
            impactName = GetStatString(customDataString, "Impact");
            eachShotReload = GetStatBool(customDataString, "EachShotReload");
        }

        private void SetStatFloat(ItemInfo info)
        {
            var customDataFloat = itemInfo
                .GetTypedData(ItemInfo.DataType.StatFloat);

            if (customDataFloat == null)
            {
                Debug.LogWarning(info.itemName + " doesn't contains Stat key");
                return;
            }

            attackRate = GetStatFloat(customDataFloat, StatKey.AttackRate.ToString());
            reloadTime = GetStatFloat(customDataFloat, StatKey.ReloadTime.ToString());
            weight = GetStatFloat(customDataFloat, StatKey.Weight.ToString());
        }

        private static int GetStat(Dictionary<string, string> customData, string statName)
        {
            return DataHandler.GetUnsafeValueInt(customData, statName);
        }

        private static string GetStatString(Dictionary<string, string> customData, string statName)
        {
            return DataHandler.GetUnsafeValue(customData, statName);
        }
        
        private static bool GetStatBool(Dictionary<string, string> customData, string statName)
        {
            var result = DataHandler.GetUnsafeValue(customData, statName);
            if (string.IsNullOrEmpty(result)) return false;
            return bool.Parse(result);
        }

        private static float GetStatFloat(Dictionary<string, string> customData, string statName)
        {
            return DataHandler.GetUnsafeValueInt(customData, statName) * 0.01f;;
        }

        public ItemInfo GetItemInfo()
        {
            return itemInfo;
        }
    }
}
