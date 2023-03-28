using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Playstel
{
    public class ShopSupportSlot : MonoBehaviour
    {
        [Header("Refs")] 
        public Image itemIcon;
        public TextMeshProUGUI itemName;
        public ShopItemInterface ShopInterface;

        public void SetItemInfo(ItemInfo itemInfo)
        {
            itemName.text = itemInfo.itemName;
            itemIcon.sprite = itemInfo.itemSprite;
            
            ShopInterface.SetItemInfo(itemInfo);
            ShopInterface.ActiveBuyButtons(true);
        }
    }
}
