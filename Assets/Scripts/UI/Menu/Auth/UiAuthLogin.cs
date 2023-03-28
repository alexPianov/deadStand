using PlayFab;
using PlayFab.ClientModels;
using Playstel.Bootstrap;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiAuthLogin : MonoBehaviour
    {
        public UiInput inputEmail;
        public UiInput inputPass;
        public Toggle rememberToggle;
        public Button Button;
        public GameObject loginPanel;
        
        [Inject] private ConnectPlayFab _connectPlayFab;

        [Inject] private BootstrapInstaller _bootstrapInstaller;

        [Inject] private HandlerLoading _handlerLoading;

        [Inject] private HandlerPulse _handlerPulse;

        private void Awake()
        {
            Button.onClick.AddListener(LoginRequest);
        }

        public void LoginRequest()
        {
            var userEmail = inputEmail.GetText();
            var userPass = inputPass.GetText();

            if (userEmail == null || userPass == null) return;
            
            _handlerLoading.OpenLoadingPopup(true, HandlerLoading.Text.Login);

            var LoginRequest = new LoginWithEmailAddressRequest()
            {
                Email = userEmail,
                Password = userPass,
                TitleId = _connectPlayFab.userSettings.GetPlayFabTitleId(),
                InfoRequestParameters = PlayFabHandler.PayloadAll()
            };

            PlayFabClientAPI.LoginWithEmailAddress(LoginRequest, OnLoginSuccess, OnLoginError);
        }

        private void OnLoginError(PlayFabError obj)
        {
            _handlerLoading.OpenLoadingPopup(false);
            _handlerPulse.OpenTextNote(obj.ErrorMessage);
        }

        private void OnLoginSuccess(LoginResult loginResult)
        {
            if (rememberToggle)
            {
                SaveRegDataToPrefs();
            }
            
            _connectPlayFab.SetLoginResult(null);
            
            loginPanel.SetActive(false);
            _handlerLoading.OpenLoadingPopup(false);
            
            _bootstrapInstaller.DataBoot();
        }
        
        private void SaveRegDataToPrefs()
        {
            PreferenceHandler.SetValue(PreferenceHandler.StringKey.Password, inputPass.GetText());
            PreferenceHandler.SetValue(PreferenceHandler.StringKey.Email, inputEmail.GetText());
        }
    }
}
