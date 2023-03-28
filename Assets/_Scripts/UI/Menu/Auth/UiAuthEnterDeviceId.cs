using Playstel.Bootstrap;
using UnityEngine;

namespace Playstel
{
    public class UiAuthEnterDeviceId : MonoBehaviour
    {
        public ConnectPlayFab ConnectPlayFab;
        public BootstrapInstaller Installer;
        public GameObject registerPanel;
        public HandlerLoading HandlerLoading;
        
        public async void EnterByDeviceId()
        {
            Debug.Log("EnterByDeviceId");
            HandlerLoading.OpenLoadingScreen(true);
            
            await ConnectPlayFab.LoginByDeviceId();

            HandlerLoading.OpenLoadingScreen(false);
            registerPanel.SetActive(false);
            
            if (ConnectPlayFab.CurrentLoginStatus == ConnectPlayFab.LoginStatus.Failed)
            {
                return;
            }
            
            Installer.DataBoot();
        }
    }
}