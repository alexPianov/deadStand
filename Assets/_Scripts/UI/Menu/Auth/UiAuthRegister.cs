using PlayFab;
using PlayFab.ClientModels;
using Playstel.Bootstrap;
using UnityEngine;
using UnityEngine.UI;

namespace Playstel
{
    public class UiAuthRegister : MonoBehaviour
    {
        [Header("Input")] 
        public UiInput inputMail;
        public UiInput inputPass;
        public UiInput inputPassRepeat;

        public Button Button;
        public GameObject registerPanel;

        public HandlerLoading HandlerLoading;
        public HandlerPulse HandlerPulse;
        public BootstrapInstaller Installer;
        public ConnectPlayFab ConnectPlayFab;

        private void Start()
        {
            Button.onClick.AddListener(RegistrationRequest);
        }

        public void RegistrationRequest()
        {
            var userMail = inputMail.GetText();
            var userPass = inputPass.GetText();
            var repeatPass = inputPassRepeat.GetText();
            var userName = userMail;
            
            Debug.Log("userName: " + userName);
            
            if (repeatPass == null || userPass == null || userMail == null || userName == null) return;
            
            if (repeatPass != userPass)
            {
                HandlerPulse.OpenTextNote("Passwords don't match");
                return;
            }
            
            HandlerLoading.OpenLoadingScreen(true, HandlerLoading.Text.Registering);
            
            var LoginRequest = new RegisterPlayFabUserRequest()
            {
                Email = userMail,
                Password = userPass,
                RequireBothUsernameAndEmail = false,
                TitleId = ConnectPlayFab.userSettings.GetPlayFabTitleId(),
                InfoRequestParameters = PlayFabHandler.PayloadAll(),
            };
            
            PlayFabClientAPI.RegisterPlayFabUser(LoginRequest, OnRegisterSuccess, OnError);
        }

        private void OnError(PlayFabError obj)
        {
            HandlerPulse.OpenTextNote(obj.ErrorMessage);
            HandlerLoading.OpenLoadingScreen(false);
        }

        private async void OnRegisterSuccess(RegisterPlayFabUserResult result)
        {
            Debug.Log("OnRegisterSuccess");
            
            SaveRegDataToPrefs();
            
            ConnectPlayFab.SetLoginResult(null);
            await ConnectPlayFab.LoginByCredentials(inputMail.GetText(), inputPass.GetText());

            registerPanel.SetActive(false);
            HandlerLoading.OpenLoadingScreen(false);
            
            Installer.DataBoot();
        }

        private void SaveRegDataToPrefs()
        {
            PreferenceHandler.SetValue(PreferenceHandler.StringKey.Password, inputPass.GetText());
            PreferenceHandler.SetValue(PreferenceHandler.StringKey.Email, inputMail.GetText());
        }
    }
}