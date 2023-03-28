using Cysharp.Threading.Tasks;
using Playstel.Bootstrap;
using UnityEngine;

namespace Playstel
{
    public class ConnectPlayFabNewlyCreated : MonoBehaviour
    {
        [Header("Panel")]
        public GameObject createAccount;

        [Header("Refs")]
        public UiCharacterNickname Nickname;
        public BootstrapInstaller Installer;
        public CacheUserInfo UserInfo;

        public bool NewlyStatus()
        {
            var newlyStatus = UserInfo.data
                .GetUserData(UserData.UserDataType.Newly);

            if (string.IsNullOrWhiteSpace(newlyStatus)) return true;
            if (string.IsNullOrEmpty(newlyStatus)) return true;

            var status = DataHandler.StringToBool(newlyStatus);
			
            return status;
        }

        public async UniTask CreateDefaultPlayerData()
        {
            await PlayFabHandler
                .ExecuteCloudScript(PlayFabHandler.Function.CreateDefaultPlayerData);
        }

        public async UniTask OpenPanel()
        {
            createAccount.SetActive(true);
            
            Installer.loading.SetLoadingText(null);
            Installer.loading.ActiveLoadingText(false);
        }

        public async void CreateAccount()
        {
            Installer.loading.OpenLoadingScreen(true);
            
            var result = await Nickname.UpdateTitleName();

            if (string.IsNullOrEmpty(result))
            {
                Debug.Log("Nickname is empty");
                Installer.loading.OpenLoadingScreen(false);
                return;
            }
            
            
            createAccount.SetActive(false);
            
            Installer.loading.SetLoadingText("Create Default Player Data");
            await CreateDefaultPlayerData();
            
            UserInfo.data.SetNewlyStatusLocally(false);
            
            Installer.loading.SetLoadingText("Game Boot");
            Installer.DataBoot();
            
            Installer.loading.OpenLoadingScreen(false);
        }
    }
}