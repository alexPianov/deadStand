using System.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using Zenject;

namespace Playstel
{
    public class ConnectPlayFab : MonoBehaviour
    {
        public UserPayload userPayload;
        public CacheUserSettings userSettings;
        public GameObject loginPanel;
        public ConnectPlayFabNewlyCreated NewlyCreated;
        public ConnectPhoton ConnectPhoton;
        public LoginMethod CurrentLoginMethod = LoginMethod.Null;
        public LoginStatus CurrentLoginStatus = LoginStatus.Null;

        [Header("UI Handlers")]
        public HandlerNetworkError HandlerNetworkError;
        public HandlerPulse HandlerPulse;
        public HandlerLoading HandlerLoading;
        
        public enum LoginMethod
        {
            Null, DeviceId, Credentials, Prefs
        }
        
        public enum LoginStatus
        {
            Null, Success, Failed
        }

        private bool _isLoaded;
        
        public void SetLoginResult(LoginResult loginResult)
        {
            _loginResult = loginResult;
        }
        
        public LoginResult GetLoginResult()
        {
            return _loginResult;
        }
        
        public async UniTask UpdateLoginResultPayload(string playFabId = null)
        {
            _loginResult.InfoResultPayload = await userPayload.UpdatePayload(playFabId);
            CurrentLoginStatus = LoginStatus.Success;
            _isLoaded = true;
        }
        
        public async UniTask LoginByPrefs()
        {
            _isLoaded = false;
            
            if (_loginResult != null)
            {
                Debug.Log("Login Result exists. Update Login Result Payload");
                
                await UpdateLoginResultPayload();
                
                return;
            }
            
            Debug.Log("Login By Prefs (Credentials)");
            
            CurrentLoginMethod = LoginMethod.Prefs;

            var email = PreferenceHandler.GetValue(PreferenceHandler.StringKey.Email);
            var password = PreferenceHandler.GetValue(PreferenceHandler.StringKey.Password);
            
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                Debug.Log("Login by device Id");
                await LoginByDeviceId();
                return;
            }

            await LoginByCredentials(email, password, false);
        }

        public async UniTask LoginByCredentials(string email = null, string password = null, bool saveLoginMethod = true)
        {
            Debug.Log("Login By Credentials: " + email);
            
            HandlerLoading.SetLoadingText("Login By Credentials");

            if (saveLoginMethod)
            {
                CurrentLoginMethod = LoginMethod.Credentials;
            }

            _isLoaded = false;
            HandlerNetworkError.StartTimer(true);

            var request = new LoginWithEmailAddressRequest()
            {
                TitleId = userSettings.GetPlayFabTitleId(),
                Email = email,
                Password = password,
                InfoRequestParameters = PlayFabHandler.PayloadAll()
            };

            PlayFabClientAPI.LoginWithEmailAddress(request,
                OnLoginSuccess, OnLoginError);
            
            await UniTask.WaitUntil(() => _isLoaded);
        }

        public async UniTask LoginByDeviceId(bool saveLoginMethod = true)
        {
            HandlerLoading.SetLoadingText("Login By Device Id");
            
            if (saveLoginMethod)
            {
                CurrentLoginMethod = LoginMethod.DeviceId;
            }
            
            _isLoaded = false;
            HandlerNetworkError.StartTimer(true);
            
#if UNITY_STANDALONE_OSX
            
            Debug.Log("Login by Os X Id: " + ReturnDeviceID());

            var requestIOS = new LoginWithIOSDeviceIDRequest
            {
                TitleId = userSettings.GetPlayFabTitleId(),
                DeviceId = ReturnDeviceID(),
                InfoRequestParameters = PlayFabHandler.PayloadAll(),
                CreateAccount = true
            };

            PlayFabClientAPI.LoginWithIOSDeviceID(requestIOS, 
                OnLoginSuccess, OnLoginError);
#endif
            
#if UNITY_ANDROID

            Debug.Log("Login by Android Id: " + ReturnDeviceID());

            var requestAndroid = new LoginWithAndroidDeviceIDRequest
            {
                TitleId = userSettings.GetPlayFabTitleId(),
                AndroidDeviceId = ReturnDeviceID(),
                InfoRequestParameters = PlayFabHandler.PayloadAll(),
                CreateAccount = true
            };

            PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid,
                OnLoginSuccess, OnLoginError);
#endif

#if UNITY_IOS
            
            Debug.Log("Login by Ios Id: " + ReturnDeviceID());
            
            var requestIOS = new LoginWithIOSDeviceIDRequest
            {
                TitleId = userSettings.GetPlayFabTitleId(),
                DeviceId = ReturnDeviceID(),
                InfoRequestParameters = PlayFabHandler.PayloadAll(),
                CreateAccount = true
            };

            PlayFabClientAPI.LoginWithIOSDeviceID(requestIOS, 
                OnLoginSuccess, OnLoginError);
#endif

            await UniTask.WaitUntil(() => _isLoaded);
        }

        private static string ReturnDeviceID()
        {
            Debug.Log("Device Id: " + SystemInfo.deviceUniqueIdentifier);
            return SystemInfo.deviceUniqueIdentifier;
        }

        private void OnLoginError(PlayFabError obj)
        {
            Debug.Log("OnLoginError: " + obj.Error);

            HandlerPulse.OpenTextNote(obj.Error.ToString());
            HandlerNetworkError.StartTimer(false);
            HandlerLoading.SetLoadingText(null);
            
            CurrentLoginStatus = LoginStatus.Failed;
            
            _isLoaded = true;

            if (obj.Error == PlayFabErrorCode.ServiceUnavailable 
                || obj.Error == PlayFabErrorCode.ConnectionError
                || obj.Error == PlayFabErrorCode.Unknown)
            {
                HandlerNetworkError.ActivePanel(true);
                return;
            }

            if (CurrentLoginMethod == LoginMethod.Prefs)
            {
                if (obj.Error == PlayFabErrorCode.AccountNotFound
                    || obj.Error == PlayFabErrorCode.AccountBanned
                    || obj.Error == PlayFabErrorCode.AccountDeleted
                    || obj.Error == PlayFabErrorCode.InvalidAccount)
                {
                    PlayerPrefs.DeleteKey(PreferenceHandler.StringKey.Email.ToString());
                    PlayerPrefs.DeleteKey(PreferenceHandler.StringKey.Password.ToString());
                }
            }
            
            OpenLoginPanel();
        }

        private LoginResult _loginResult;
        private async void OnLoginSuccess(LoginResult result)
        {
            Debug.Log("OnLoginSuccess");
            
            CurrentLoginStatus = LoginStatus.Success;
            
            HandlerNetworkError.StartTimer(false);
            SetLoginResult(result);

            await userPayload.ReceivePayload(result.InfoResultPayload);
            
            await ConnectPhoton.SetPlayFabAuthValues(result.AuthenticationContext);
            
            if (NewlyCreated.NewlyStatus())
            {
                NewlyCreated.OpenPanel();
            }
            
            _isLoaded = true;
        }
        
        public void OpenLoginPanel()
        {
            Debug.Log("OpenLoginPanel");
            HandlerLoading.OpenLoadingScreen(false);
            loginPanel.SetActive(true);
        }
    }
}
