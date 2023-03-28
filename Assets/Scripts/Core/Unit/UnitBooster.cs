using System;
using EventBusSystem;
using Photon.Pun;
using UnityEngine;
using Zenject;
using static Playstel.EnumStore;

namespace Playstel
{
    public class UnitBooster : MonoBehaviourPun
    {
        [Inject] private CacheItemInfo _cacheItemInfo;

        private Unit _unit;
        private void Start()
        {
            _unit = GetComponent<Unit>();
        }

        public void TakeBooster(ItemBooster booster, Transform transform)
        {
            Debug.Log("Take Booster");

            var boosterItemInfo = _cacheItemInfo.GetItemInfo
                (booster.ToString(), ItemInfo.Catalog.Support);

            var boosterItem = ItemHandler.CreateItem(boosterItemInfo);
            
            if (boosterItem == null) return;

            if (booster.boosterType == BoosterType.Ammo)
            {
                _unit.Ammo.Add(boosterItem); return;
            }

            if (booster.boosterType == BoosterType.Drug)
            {
                _unit.Drugs.Use(boosterItem);
            }
        }
    }
}