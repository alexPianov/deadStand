using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    [RequireComponent(typeof(Button))]
    public class UIDialogButtonToken : UIDialogButton
    {
        public TextMeshProUGUI itemCount;

        [Inject] private Unit _unit;
        [Inject] private CacheAudio _cacheAudio;

        private string _requiredItem;

        public void Awake()
        {
            GetComponent<Button>().onClick.AddListener(SellAllTokens);
        }

        public void UpdateTokenDisplay()
        {
            var tokensCount = _unit.Tokens.GetTokensCount();

            if (tokensCount == 0)
            {
                gameObject.SetActive(false); return;
            }

            itemCount.text = "x" + tokensCount;
        }

        public async void SellAllTokens()
        {
            if(!_unit.photonView.IsMine) return;
            
            var tokensCount = _unit.Tokens.GetTokensCount();

            var request = HandlerHostRequest
                .GetTokenRequest(tokensCount, true);

            await _unit.HostOperator
                .Run(UnitHostOperator.Operation.Token, request);

            _cacheAudio.Play(CacheAudio.MenuSound.OnLootDrop);
            
            UpdateTokenDisplay();
        }
    }
}

