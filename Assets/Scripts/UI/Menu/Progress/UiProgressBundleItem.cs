using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiProgressBundleItem : MonoBehaviour
    {
        [Header("Common")]
        public Image itemImage;
        public Image itemLockImage;
        
        [Header("Button")]
        public Button itemButton;
        
        [Inject] private CacheSprites _cacheSprites;
        
        private void Awake()
        {
            itemButton.onClick.AddListener(ShowItem);
        }

        private UiProgressItemShow _itemShow;
        public void SetItemDemo(UiProgressItemShow itemShow)
        {
            _itemShow = itemShow;
        }

        private void ShowItem()
        {
            Debug.Log("Show Item " + _item.info.itemName);
            _itemShow.OpenItem(_item);
        }

        private Item _item;
        public void SetItem(Item item, bool isLocked)
        {
            _item = item;
            itemLockImage.enabled = isLocked;
            itemImage.sprite = GetItemSprite(item.info);
        }

        public void BattlePassLock(bool battlePass, bool isLocked)
        {
            if(isLocked) return;
            itemLockImage.enabled = !battlePass;
        }
        
        private Sprite GetItemSprite(ItemInfo item)
        {
            return _cacheSprites.GetSpriteFromAtlas(item.itemName, item.itemCatalog);
        }
    }
}