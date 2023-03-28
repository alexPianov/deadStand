using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using EventBusSystem;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UnitTokens : MonoBehaviourPun
    {
        public ObscuredInt pickedTokens;
        public GameObject uiTokenPrefab;
        private Unit _unit;

        [Inject] private LocationInstaller _locationInstaller;
        [Inject] private CacheItemInfo _cacheItemInfo;
        [Inject] private CacheAudio _cacheAudio;
        
        private string tokenName = "FragToken";

        private void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        public void UpdateTokens(int value)
        {
            pickedTokens = value;
            UpdateUi();
        }

        public int GetTokensCount()
        {
            return pickedTokens;
        }

        public int GetTokensPrice()
        {
            var tokenPrice = _cacheItemInfo
                .GetItemInfo(tokenName, ItemInfo.Catalog.Setup, ItemInfo.Class.Loot)
                .GetItemPrice();

            return tokenPrice * GetTokensCount();
        }
        
        public void UpdateUi()
        {
            if(!photonView.IsMine || _unit.IsNPC) return;

            EventBus.RaiseEvent<ITokenHandler>
                (h => h.HandleValue(pickedTokens));
        }

        public async void CreateToken()
        {
            var token = await _locationInstaller.LoadElement<GameObject>(tokenName);
            token.transform.position = transform.position;

            if (token.TryGetComponent(out LootFragToken fragToken))
            {
                fragToken.SetFraction(_unit.Fraction.currentFraction);
            }
        }

        public async void TakeToken(Transform tokenTransform)
        {
            if (!_unit.IsNPC)
            {
                _cacheAudio.Play(CacheAudio.MenuSound.OnTakeToken);

                EventBus.RaiseEvent<IAttractorUi>(h => h.HandlerAttract(tokenTransform));

                await UniTask.Delay(650);
            }

            var request = HandlerHostRequest.GetTokenRequest(pickedTokens);

            _unit.HostOperator.Run(UnitHostOperator.Operation.Token, request);
        }
    }
}
