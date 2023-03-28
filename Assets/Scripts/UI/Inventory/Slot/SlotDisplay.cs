using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Playstel
{
    public class SlotDisplay : MonoBehaviour
    {
        [Header("Refs")] 
        public Image itemIcon;
        public TextMeshProUGUI itemPrice;

        public void SetItem(Item item)
        {
            SetSprite(item.info.itemSprite);
            SetPrice(item);
        }

        private void SetSprite(Sprite sprite)
        {
            itemIcon.enabled = sprite != null;
            itemIcon.sprite = sprite;
        }

        private void SetPrice(Item item)
        {
            var price = item.info.GetItemPrice(ItemInfo.Currency.BC);
            itemPrice.enabled = price != 0;
            itemPrice.text = price.ToString();
        }
    }
}
