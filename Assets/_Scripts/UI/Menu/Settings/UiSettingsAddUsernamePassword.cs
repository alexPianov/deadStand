using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiSettingsAddUsernamePassword : MonoBehaviour
    {
        [Header("Input")]
        public UiInput inputMail;
        public UiInput inputPass;
        public UiInput inputPassRepeat;

        public Button Button;
        public UiSettingsRegister Register;

        [Inject] private HandlerLoading _handlerLoading;
        [Inject] private CacheUserInfo _cacheUserInfo;
        [Inject] private HandlerPulse _handlerPulse;

        private void Start()
        {
            Button.onClick.AddListener(RegistrationRequest);
        }

        public void RegistrationRequest()
        {
            var userMail = inputMail.GetText();
            var userPass = inputPass.GetText();
            var repeatPass = inputPassRepeat.GetText();
            var userName = _cacheUserInfo.payload.GetTitleDisplayName();
            
            Debug.Log("userName: " + userName);
            if (repeatPass == null || userPass == null || userMail == null || userName == null) return;
            
            if (repeatPass != userPass)
            {
                _handlerPulse.OpenTextNote("Passwords don't match");
                return;
            }
            
            _handlerLoading.OpenLoadingPopup(true, HandlerLoading.Text.Registering);
            
            var AddUsernamePasswordRequest = new AddUsernamePasswordRequest()
            {
                Email = userMail,
                Password = userPass,
                Username = userName
            };

            PlayFabClientAPI.AddUsernamePassword(AddUsernamePasswordRequest, 
                OnAddUsernamePassword, OnError);
        }

        private void OnError(PlayFabError obj)
        {
            _handlerPulse.OpenTextNote(obj.ErrorMessage);
            _handlerLoading.OpenLoadingPopup(false);
        }

        private void OnAddUsernamePassword(AddUsernamePasswordResult result)
        {
            Debug.Log("OnRegisterSuccess");
            
            SaveRegDataToPrefs();
            
            _handlerPulse.OpenTextNote("Credentials saved");
            _handlerLoading.OpenLoadingPopup(false);

            if (Register)
            {
                Register.SetEmail(inputMail.GetText());
                Register.OpenRegisterPanel(false);
            }
        }

        private void SaveRegDataToPrefs()
        {
            PreferenceHandler.SetValue(PreferenceHandler.StringKey.Password, inputPass.GetText());
            PreferenceHandler.SetValue(PreferenceHandler.StringKey.Email, inputMail.GetText());
        }
    }
}
