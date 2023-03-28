using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using EventBusSystem;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace Playstel
{
    public class UnitCurrency : MonoBehaviourPun
    {
        private Dictionary<ItemInfo.Currency, ObscuredInt> _currencyStore = new ();

        private Unit _unit;
        private void Start()
        {
            _unit = GetComponent<Unit>();
        }

        public void UpdateCurrency(int amount, string currencyName, bool updateUi = true)
        {
            var currency = DataHandler.GetCurrencyType(currencyName);

            if (_currencyStore.ContainsKey(currency))
            {
                _currencyStore.Remove(currency);
            }

            _currencyStore.Add(currency, amount);
            
            if(updateUi) UpdateUi(currency);
        }

        public int GetUnitCurrency(ItemInfo.Currency currency)
        {
            _currencyStore.TryGetValue(currency, out var value);
            return value;
        }

        public void UpdateUi(ItemInfo.Currency currency)
        {
            if(!photonView.IsMine || _unit.IsNPC) return;

            if (currency == ItemInfo.Currency.BC)
            {
                _currencyStore.TryGetValue(currency, out var value);
                EventBus.RaiseEvent<IBottleCapsHandler>(h => h.HandleValue(value));
            }

            if (currency == ItemInfo.Currency.GL)
            {
                _currencyStore.TryGetValue(currency, out var value);
                EventBus.RaiseEvent<IGoldHandler>(h => h.HandleValue(value));
            }
        }
    }
}
