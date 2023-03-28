using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Playstel
{
    public class ShopWeaponSlotCurrency : MonoBehaviour
    {
        public ItemInfo.Currency currentCurrency;
        public TextMeshProUGUI itemPrice;

        public void SetItemInfo(ItemInfo itemInfo)
        {
            if (itemInfo == null)
            {
                gameObject.SetActive(false);
                return;
            }
            
            var price = itemInfo.GetItemPrice(currentCurrency);
            
            gameObject.SetActive(price != 0);
            itemPrice.text = price.ToString();
        }
    }
}
