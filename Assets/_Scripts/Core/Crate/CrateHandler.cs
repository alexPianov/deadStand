using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Playstel
{
    public class CrateHandler : MonoBehaviour
    {
        [Inject]
        private CacheItemInfo _cacheItemInfo;
        
        private Crate _crate;

        private void Awake()
        {
            _crate = GetComponent<Crate>();
        }

        public List<string> GetRandomCrateItemNames()
        {
            var itemList = _cacheItemInfo
                .CreateItemList(ItemInfo.Catalog.Backpack, ItemInfo.Class.Loot);

            var itemNameList = new List<string>();
            
            for (int i = 0; i < GetItemCountInCrate(); i++)
            {
                var item = itemList[Random.Range(0, itemList.Count)];
                itemNameList.Add(item.info.itemName);
            }

            return itemNameList;
        }
        
        public string GetRandomCrateInstanceName()
        {
            var chance = Random.Range(1, 100);
            var names = _crate.Stat.crateInstanceNames;

            if (chance <= _crate.Stat.spawnProbability)
            {
                var crateName = names[Random.Range(0, names.Count)];
                
                if (string.IsNullOrEmpty(crateName))
                {
                    Debug.Log("Crate name is null");
                    return names[0];
                }
                
                return crateName;
            }

            return null;
        }

        public int GetItemCountInCrate()
        {
            var itemCount = Random.Range(_crate.Stat.minItems, _crate.Stat.maxItems);
            return itemCount;
        }

        public int GetRespawnCooldown()
        {
            return _crate.Stat.respawnCooldown;
        }

        public int GetCrateSize()
        {
            return _crate.Stat.crateSize;
        }
    }
}