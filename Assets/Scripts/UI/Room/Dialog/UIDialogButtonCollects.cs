using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    [RequireComponent(typeof(Button))]
    public class UIDialogButtonCollects : UIDialogButton
    {
        public Image collectIcon;
        public TextMeshProUGUI itemCount;

        [Inject] private Unit _unit;
        [Inject] private CacheAudio _cacheAudio;

        private string _requiredItem;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(SellCollectItems);
        }

        public void UpdateCollectItem(string itemName)
        {
            _requiredItem = itemName;
            
            var items = _unit.Items.GetSameItems(itemName);

            if (items == null || items.Count == 0)
            {
                gameObject.SetActive(false); return;
            }

            collectIcon.sprite = items[0].info.itemSprite;
            itemCount.text = "x" + items.Count;
        }

        public async void SellCollectItems()
        {
            Debug.Log("SellCollectItems");
            
            var items = _unit.Items.GetSameItems(_requiredItem);

            var request = HandlerHostRequest
                .GetSellRequest(items, ItemInfo.Catalog.Backpack, true);

            await _unit.HostOperator.Run(UnitHostOperator.Operation.Sell, request);
            
            _cacheAudio.Play(CacheAudio.MenuSound.OnLootDrop);

            UpdateCollectItem(_requiredItem);
        }
    }
}

