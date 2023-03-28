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
    public class UiSettingsLogin : MonoBehaviour
    {
        public Button loginButton;
        
        [Inject] private ConnectPlayFab _connectPlayFab;

        private void Start()
        {
            loginButton.onClick.AddListener(OpenLogin);
        }

        public void OpenLogin()
        {
            _connectPlayFab.OpenLoginPanel();
        }
    }
}