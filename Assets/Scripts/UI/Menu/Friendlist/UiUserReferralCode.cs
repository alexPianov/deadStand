using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;
using Zenject;
using Button = UnityEngine.UI.Button;

namespace Playstel
{
    public class UiUserReferralCode : MonoBehaviour
    {
        [Header("Widget")] 
        public GameObject referralWidget;

        [Header("Buttons")]
        public Button copyReferralCode;
        public Button inputReferralCode;
        public Button sendReferralCode;
        
        [Header("Input")]
        public GameObject inputPanel;
        public UiInput inputName;
        
        [Header("Refs")]
        public UiFriends UiFriends;

        private const int referrerGoldReward = 50;

        [Inject] private CacheUserInfo _cacheUserInfo;
        [Inject] private CacheUserFriends _cacheUserFriends;

        [Inject] private HandlerPulse _handlerPulse;
        [Inject] private HandlerLoading _handlerLoading;

        private void Start()
        {
            copyReferralCode.onClick.AddListener(CopyReferralCode);
            inputReferralCode.onClick.AddListener(OpenInputPanel);
            sendReferralCode.onClick.AddListener(SendCode);
        }

        private void CopyReferralCode()
        {
            var playFabID = _cacheUserInfo.payload.GetPlayFabId();
            UniClipboard.SetText(playFabID);
            _handlerPulse.OpenTextNote("Referral code is copied");
            
            Debug.Log("Get From Clipboard: " + UniClipboard.GetText());
        }

        public void OpenReferralWidget(bool state)
        {
            referralWidget.SetActive(state);
        }

        private void OpenInputPanel()
        {
            ActiveInputPanel(true);
        }

        private async void SendCode()
        {
            var userPlayFabId = inputName.GetText();

            if (userPlayFabId == null) return;

            if (userPlayFabId == _cacheUserInfo.payload.GetPlayFabId())
            {
                _handlerPulse.OpenTextNote("You can't send your code to yourself");
                return;
            }

            if (CheckFriend(userPlayFabId))
            {
                _handlerPulse.OpenTextNote("You can't send your code to your friend");
                return;
            }

            _handlerLoading.OpenLoadingPopup(true, HandlerLoading.Text.Sending);
            
            var args = new
            {
                referralCode = userPlayFabId
            };
            
            var result = await PlayFabHandler.ExecuteCloudScript
                (PlayFabHandler.Function.RedeemReferral, args);

            if (result.FunctionResult == null)
            {
                _handlerPulse.OpenTextNote("Code Activated! You just received " + referrerGoldReward + " gold");
                UiFriends.ShowUsers(UiFriends.Status.Confirmed, true);
                ActiveInputPanel(false);
                await UniTask.Delay(1000);
                UiFriends.Refresh();
            }
            else
            {
                _handlerPulse.OpenTextNote(result.FunctionResult.ToString());
            }
            
            _handlerLoading.OpenLoadingPopup(false);
        }

        private bool CheckFriend(string userPlayFabId)
        {
            foreach (var friend in _cacheUserFriends.GetPlayFabFriends())
            {
                if (friend.FriendPlayFabId == userPlayFabId) return true;
            }

            return false;
        }

        public void ActiveInputPanel(bool state)
        {
            inputPanel.SetActive(state);
        }
    }
}
