using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class ShopBuyButton : MonoBehaviour
    {
        [Header("Refs")] 
        public ItemInfo.Currency buttonCurrency;
        public TextMeshProUGUI itemPrice;
        public Button button;

        private ItemInfo _currentItem;
        
        [Inject] private Unit _unit;
        [Inject] private CacheAudio _cacheAudio;
        [Inject] private HandlerPulse _handlerPulse;

        private void Awake()
        {
            button.onClick.AddListener(Purchase);
        }

        public void SetBuyButton(ItemInfo itemInfo)
        {
            if(!_unit.photonView.IsMine) return;
            
            _currentItem = itemInfo;
            
            var price = itemInfo.GetItemPrice(buttonCurrency);
            
            gameObject.SetActive(price != 0);
            itemPrice.text = price.ToString();

            CheckItemAvailability();
        }

        private void CheckItemAvailability()
        {
            if (_currentItem.itemClass == ItemInfo.Class.Ammo)
            {
                Interactible(true);
                return;
            }
            
            var unitItems = _unit.Items.GetItemList();

            var dupes = unitItems
                .FindAll(item => item.info.itemName == _currentItem.itemName);
            
            Interactible(dupes.Count == 0);
        }

        private async void Purchase()
        {
            if(!_unit.photonView.IsMine) return;
            
            Interactible(false);

            var request = HandlerHostRequest
                .GetBuyRequest(_currentItem, buttonCurrency, _currentItem.itemCatalog);
            
            var result = await _unit.HostOperator
                .Run(UnitHostOperator.Operation.Buy, request);

            if (result == UnitHostOperator.Status.Success)
            {
                _cacheAudio.Play(CacheAudio.MenuSound.OnPurchase);
                
                if (GetComponent<UIElementLoadByType>())
                {
                    _cacheAudio.Play(CacheAudio.MenuSound.OnTakeWeapon);
                    
                    await GetComponent<UIElementLoadByType>().Load
                        (UIElementLoad.Elements.GUI, UIElementContainer.Type.Screen);
                }
            }
            else
            {
                _cacheAudio.Play(CacheAudio.MenuSound.OnError);
                _handlerPulse.OpenTextNote("Insufficient Funds");
            }

            CheckItemAvailability();
        }

        public void Interactible(bool state)
        {
            if(button) button.interactable = state;
        }
    }
}
