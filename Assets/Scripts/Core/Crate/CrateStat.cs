using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class CrateStat : MonoBehaviour
    {
        private ItemInfo _crateInfo;
        
        public ObscuredInt respawnCooldown;
        public ObscuredInt spawnProbability;
        public ObscuredInt maxItems;
        public ObscuredInt minItems;
        public ObscuredInt crateSize;
        public List<ObscuredString> crateInstanceNames;
        
        [Inject] private CacheItemInfo _cacheItemInfo;
        
        private const string postfix = "Crate";
        public void Install(CrateBoot.Type crateType)
        {
            _crateInfo = _cacheItemInfo.GetItemInfo
                (crateType + postfix, ItemInfo.Catalog.Setup, ItemInfo.Class.Crate);

            if (_crateInfo == null)
            {
                Debug.LogError("Failed to find crate info: " + crateType + postfix);
                return;
            }

            minItems = GetValue("MinItems");
            maxItems = GetValue("MaxItems");
            spawnProbability = GetValue("Probability");
            respawnCooldown = GetValue("Cooldown");
            crateSize = GetValue("CrateSize");
            crateInstanceNames = GetSplitString("Names");
        }

        private int GetValue(string key)
        {
            return int.Parse(_crateInfo.GetUnsafeValue(key));
        }

        private List<ObscuredString> GetSplitString(string key)
        {
            var crateNamesString = _crateInfo.GetUnsafeValue(key);

            var crateNamesUnsafe = DataHandler
                .SplitString(crateNamesString);

            return DataHandler.ConvertToSafeData(crateNamesUnsafe);
        }
    }
}
