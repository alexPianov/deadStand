 using EasyUI.PickerWheelUI;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EventBusSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiRoulette : MonoBehaviour
    {
        [Header("Refs")] 
        public PickerWheel Wheel;
        public Button Button;
        public Button CloseButton;
        public TextMeshProUGUI SpinPrice;
        public UiTransparency PrizeArrow;
        public UiRouletteReward UiRouletteReward;
        
        [Inject] private Unit _unit;
        [Inject] private HandlerPulse _handlerPulse;
        [Inject] private CacheTitleData _cacheTitleData;
        [Inject] private CacheItemInfo _cacheItemInfo;
        [Inject] private CacheAudio _cacheAudio;

        private void Start()
        {
            Button.onClick.AddListener(Spin);
            InitializeWheelPieces();
        }

        private const int wheelItemsCount = 8;
        private ItemInfo.Catalog wheelItemsCatalog = ItemInfo.Catalog.Weapons;
        private ItemInfo.Class wheelItemsClass = ItemInfo.Class.Firearm;
        private void InitializeWheelPieces()
        {
            if (_cacheTitleData == null)
            {
                Debug.Log("Failed to find cache title data"); return;
            }

            SetSpinPriceText();
            
            var casinoItems = GetCasinoItems();

            var wheelPieces = CreateWheelPieces(casinoItems);

            Wheel.AddWheelPieces(wheelPieces.ToArray());
        }

        private void SetSpinPriceText()
        {
            SpinPrice.text = _cacheTitleData
                .GetTitleDataInt(CacheTitleData.TitleDataKey.SpinPrice).ToString();
        }

        private List<ItemInfo> GetCasinoItems()
        {
            List<ItemInfo> casinoItems = new List<ItemInfo>();

            var weaponItems = _cacheItemInfo
                .GetItemInfoList(wheelItemsCatalog, wheelItemsClass);

            for (int i = 0; i < wheelItemsCount; i++)
            {
                var randomItem = weaponItems[Random.Range(0, weaponItems.Count)];
                casinoItems.Add(randomItem);
            }

            return casinoItems;
        }

        private static List<WheelPiece> CreateWheelPieces(List<ItemInfo> items)
        {
            var wheelPieces = new List<WheelPiece>();

            foreach (var item in items)
            {
                var piece = new WheelPiece();
                piece.SetItemInfo(item);
                wheelPieces.Add(piece);
            }

            return wheelPieces;
        }

        private bool spinProcess;
        private void Spin()
        {
            if (spinProcess) return;

            var spinPrice = _cacheTitleData
                .GetTitleDataInt(CacheTitleData.TitleDataKey.SpinPrice);
            
            var unitCurrency = _unit.Currency.GetUnitCurrency(ItemInfo.Currency.BC);
            
            if(unitCurrency < spinPrice)
            {
                _cacheAudio.Play(CacheAudio.MenuSound.OnError);
                _handlerPulse.OpenTextNote("Insufficient Funds"); return;
            }
            
            _cacheAudio.Play(CacheAudio.MenuSound.OnPurchase);
            
            var newCurrencyValue = unitCurrency - spinPrice;
                
            EventBus.RaiseEvent<IBottleCapsHandler>
                (h => h.HandleValue(newCurrencyValue));

            PrizeArrow.Transparency(true);
            CloseButton.interactable = false;
            Button.interactable = false;

            Wheel.Spin();
            Wheel.onSpinEndEvent += Finish;
        }

        public async void Finish(WheelPiece piece)
        {
            PrizeArrow.Transparency(false);
            _cacheAudio.Play(CacheAudio.MenuSound.OnProcess);

            await UniTask.Delay(1000);

            UiRouletteReward.ShowReward(piece.GetItemInfo());
            
            _cacheAudio.Play(CacheAudio.MenuSound.OnGetItem);
            
            PrizeArrow.Transparency(true);
            CloseButton.interactable = true;
            Button.interactable = true;
        }
    }
}
