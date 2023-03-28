using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class ShopItemInterface: MonoBehaviour
    {
        public Transform statLayout;
        public Transform buttonsLayout;

        private List<ShopBuyButton> buttonsList = new ();
        private List<ShopItemStat> statList = new ();

        private ItemInfo _itemInfo;
        private Unit _unit;

        private void Awake()
        {
            statList = statLayout.GetComponentsInChildren<ShopItemStat>().ToList();
            
            if (buttonsLayout)
            {
                buttonsList = buttonsLayout.GetComponentsInChildren<ShopBuyButton>().ToList();
            }
        }

        public ItemInfo GetCurrentItemInfo()
        {
            return _itemInfo;
        }

        public void SetItemInfo(ItemInfo itemInfo)
        {
            if(_itemInfo == itemInfo) return;
            
            _itemInfo = itemInfo;
            
            SetStatDisplay(itemInfo);
        }

        public void ActiveBuyButtons(bool state)
        {
            if(!buttonsLayout) return;
            
            foreach (var button in buttonsList)
            {
                if (state)
                {
                    button.SetBuyButton(_itemInfo);
                }
                else
                {
                    button.gameObject.SetActive(state);
                }
            }
        }
        
        private void SetStatDisplay(ItemInfo info)
        {
            foreach (var stat in statList)
            {
                stat.SetValue(info);
            }
        }
    }
}