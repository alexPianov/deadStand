using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    public class CacheBoosters : MonoBehaviour
    {
        public List<ItemBooster> boosterList;

        public ItemBooster GetBooster(string boosterName)
        {
            return boosterList.Find(booster => booster.boosterName.ToString() == boosterName);
        }

        public ItemBooster GetRandomBooster()
        {
            if (boosterList.Count == 0) return null;
            return boosterList[Random.Range(0, boosterList.Count)];
        }
    }
}