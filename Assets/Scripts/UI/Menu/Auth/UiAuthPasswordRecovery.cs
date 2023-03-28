using System;
using CodeStage.AntiCheat.ObscuredTypes;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiAuthPasswordRecovery : MonoBehaviour
    {
        public UiInput inputEmail;
        public Button restoreButton;
        
        [Inject] private CacheUserSettings _cacheUserSettings;
        [Inject] private HandlerPulse _handlerPulse;
        [Inject] private HandlerLoading _handlerLoading;

        private void Awake()
        {
            restoreButton.onClick.AddListener(RestorePassword);
        }

        public void RestorePassword()
        {
            var email = inputEmail.GetText();
            
            if(email == null) return;
            
            _handlerLoading.OpenLoadingPopup(true, HandlerLoading.Text.Restoring);
            
            var RecoveryRequest = new SendAccountRecoveryEmailRequest()
            {
                Email = email,
                TitleId = _cacheUserSettings.GetPlayFabTitleId()
            };

            PlayFabClientAPI.SendAccountRecoveryEmail(RecoveryRequest, 
                result =>
                {
                    gameObject.SetActive(false);
                    _handlerLoading.OpenLoadingPopup(false);
                    _handlerPulse.OpenTextNote("Account recovery was sent to your Email");
                },
                error =>
                {
                    _handlerLoading.OpenLoadingPopup(false);
                    _handlerPulse.OpenTextNote("Failed to send account recovery");
                });
        }
    }
}