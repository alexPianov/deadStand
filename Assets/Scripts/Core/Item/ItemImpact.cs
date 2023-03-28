using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    public class ItemImpact : ScriptableObject
    {
        [HideInInspector]
        public ItemInfo itemInfo;
        public ObscuredInt hostId;

        [Header("Impact")] 
        public ObscuredInt impactDamage;
        public ObscuredInt impactDuration;
        public ObscuredInt additionDamage;
        public ObscuredFloat speedPenalty;
        public ObscuredFloat impactDamageRate;

        [Header("Field")]
        public ObscuredInt fieldRange;
        public ObscuredInt fieldLifetime;
        public ObscuredFloat fieldExpandSpeed;
        public ObscuredFloat fieldRate;
        public ObscuredBool blasting;

        [Header("FX")]
        public ObscuredString effectOrigin;
        public ObscuredString effectExpand;
        public ObscuredString soundOrigin;
        public ObscuredString soundExpand;

        public void SetHostId(int id)
        {
            hostId = id;
        }

        public void SetInfo(ItemInfo info)
        {
            itemInfo = info;
            
            SetStatInt();
            SetStatFloat();
            SetStatString();
            SetFX();
        }

        private void SetStatString()
        {
            var blastStatus = itemInfo
                .GetTypedData(ItemInfo.DataType.StatString, "Blasting");

            blasting = bool.Parse(blastStatus);
        }

        private void SetStatFloat()
        {
            var customDataFloat = itemInfo
                .GetTypedData(ItemInfo.DataType.StatFloat);

            fieldExpandSpeed = GetStatFloat(customDataFloat, "FieldExpandSpeed");
            speedPenalty = GetStatFloat(customDataFloat, "SpeedPenalty");
            
            fieldRate = GetStatFloat(customDataFloat, "FieldRate");
            impactDamageRate = GetStatFloat(customDataFloat, "ImpactDamageRate");
        }

        private void SetStatInt()
        {
            var customDataInt = itemInfo
                .GetTypedData(ItemInfo.DataType.StatInt);

            fieldRange = GetStat(customDataInt, "FieldRange");
            fieldLifetime = GetStat(customDataInt, "FieldLifetime");
            
            impactDamage = GetStat(customDataInt, "ImpactDamage");
            impactDuration = GetStat(customDataInt, "ImpactDuration");
            additionDamage = GetStat(customDataInt, "AdditionDamage");
        }

        private void SetFX()
        {
            var customData = itemInfo.GetTypedData(ItemInfo.DataType.FX);

            effectOrigin = GetStatString(customData, "V_Orig");
            effectExpand = GetStatString(customData, "V_Expd");

            soundOrigin = GetStatString(customData, "S_Orig");
            soundExpand = GetStatString(customData, "S_Expd");
        }

        private int GetStat(Dictionary<string, string> customData, string statName)
        {
            return DataHandler.GetUnsafeValueInt(customData, statName);
        }

        private string GetStatString(Dictionary<string, string> customData, string statName)
        {
            return DataHandler.GetUnsafeValue(customData, statName);
        }

        private float GetStatFloat(Dictionary<string, string> customData, string statName)
        {
            return DataHandler.GetUnsafeValueInt(customData, statName) * 0.01f;
        }
    }
}
