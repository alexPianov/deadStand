using System;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Zenject;


namespace Playstel
{
    public class UiCharacterNickname : MonoBehaviour
    {
        public UiInput nicknameInput;
        public GameObject inputSceen;

        [Inject]
        private HandlerLoading _handlerLoading;
        
        [Inject]
        private HandlerPulse _handlerPulse;

        [HideInInspector]
        public string characterName;

        public async void ConfirmButton()
        {
            characterName = await UpdateTitleName();
            
            if(string.IsNullOrEmpty(characterName)) return;
            
            ActivePanel(false);
        }

        public void ActivePanel(bool state)
        {
            if(inputSceen) inputSceen.SetActive(state);
            
            if(GetComponent<UiUnitRendererOnStart>())
                GetComponent<UiUnitRendererOnStart>().ActiveRenderer(!state);
        }

        public async UniTask<string> UpdateTitleName()
        {
            var userName = nicknameInput.GetText();
            
            if(string.IsNullOrEmpty(userName)) return null;

            _handlerLoading.OpenLoadingScreen(true, HandlerLoading.Text.Saving);
            
            var result = await PlayFabHandler
                .SetTitleName(userName, _handlerPulse);
            
            _handlerLoading.OpenLoadingScreen(false);

            if (result == null)
            {
                Debug.Log("Failed to update title name"); return null;
            }
            
            UpdatePhotonNickname(result.DisplayName);
            
            return result.DisplayName;
        }

        public void SetCurrentNickname(string nickName)
        {
            nicknameInput.GetComponent<TMP_InputField>().text = nickName;
        }
        
        private void UpdatePhotonNickname(string displayName)
        {
            Debug.Log("Set User Name to Photon Network: " + displayName);
            PhotonNetwork.NickName = displayName;
        }
    }
}
