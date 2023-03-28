using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiRouletteReward : MonoBehaviour
    {
        [Header("Refs")]
        public ShopWeaponInterface ShopWeaponInterface;
        private const ItemInfo.Catalog itemCatalog = ItemInfo.Catalog.Weapons;

        [Header("Buttons")]
        public Button takeButton;
        public Button discardButton;

        [Inject] private Unit _unit;
        [Inject] private HandlerPulse _handlerPulse;
        [Inject] private HandlerLoading _handlerLoading;

        private UIElementLoadByType _uiElementLoadByType;

        private void Awake()
        {
            _uiElementLoadByType = GetComponent<UIElementLoadByType>();
            
            takeButton.onClick.AddListener(TakeReward);
            discardButton.onClick.AddListener(DiscardReward);
        }

        private ItemInfo _rewardItemInfo;
        public void ShowReward(ItemInfo itemInfo)
        {
            _rewardItemInfo = itemInfo;
            
            ShopWeaponInterface.SetItemInfo(itemInfo);
            ShopWeaponInterface.ActiveWeaponPanel(true);
            ShopWeaponInterface.SetWeaponName(itemInfo);
        }

        private async void TakeReward()
        {
            if (!HasDupe())
            {
                await EndIteration(_rewardItemInfo.itemName);
            }
            
            _uiElementLoadByType.Load(UIElementLoad.Elements.GUI, UIElementContainer.Type.Screen);
        }

        private bool HasDupe()
        {
            var items = _unit.Items.GetSameItems(_rewardItemInfo.itemName);
            return items.Count > 0;
        }

        private async void DiscardReward()
        {
            await EndIteration("");
            
            _uiElementLoadByType.Load(UIElementLoad.Elements.Dialog, UIElementContainer.Type.Screen);
        }
        
        public async UniTask EndIteration(string itemName)
        {
            if(!_unit.photonView.IsMine) return;
            
            _handlerLoading.OpenLoadingPopup(true, HandlerLoading.Text.Check);
            
            var request = HandlerHostRequest
                .GetCasinoSpinRequest(itemName, itemCatalog, ItemInfo.Currency.BC);

            var result = await _unit.HostOperator.Run(UnitHostOperator.Operation.Spin, request);

            if (result == UnitHostOperator.Status.Reject)
            {
                _handlerPulse.OpenTextNote("Roulette Error");
            }
            
            _handlerLoading.OpenLoadingPopup(false);
        }
    }
}