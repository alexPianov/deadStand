using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Playstel
{
    [RequireComponent(typeof(Button))]
    public class ShopWeaponSlot : MonoBehaviour
    {
        [Header("Refs")] 
        public Image itemIcon;
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI itemSubclass;
        public Transform currencyLayout;
        public GameObject focus;

        private ItemInfo _currentItemInfo;
        private ShopFactory _shopFactory;
        private ShopWeaponInterface _weaponInterface;
        private Transform _focus;
        private ShopWeaponSlotCurrency currency;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(UpdateInterface);
        }

        public void SetItemInfo(ItemInfo itemInfo, ShopFactory factory = null)
        {
            _currentItemInfo = itemInfo;
            _shopFactory = factory;
            
            if(factory) _focus = factory.focus;

            itemName.text = itemInfo.itemName;
            itemSubclass.text = itemInfo.ItemSubclass.ToString();
            itemIcon.sprite = itemInfo.itemSprite;

            currency.SetItemInfo(itemInfo);
            
            SetFocus(IsAvailable(itemInfo));  
        }

        private bool IsAvailable(ItemInfo item)
        {
            var result = _shopFactory.GetUnit().Items
                .GetSameItems(item.itemName);

            return result.Count > 0;
        }

        public void SetInterface(ShopWeaponInterface weaponInterface)
        {
            _weaponInterface = weaponInterface;
        }
        
        public void UpdateInterface()
        {
            _weaponInterface.SetItemInfo(_currentItemInfo);
            _weaponInterface.ActiveWeaponPanel(true);
            _weaponInterface.SetWeaponName(_currentItemInfo);

            _weaponInterface.ActiveBuyButtons(!IsAvailable(_currentItemInfo));
        }

        public void SetFocus(bool state)
        {
            focus.SetActive(state);
        }
    }
}
