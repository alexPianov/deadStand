using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiUserSearch : UiFactory
    {
        public UiUserInfoHandle UserInfoHandle;
        public UiFriends UiFriends;
        
        [Header("Refs")]
        public GameObject searchPanel;
        public UiInput inputName;
        public Button buttonFind;

        [Inject] private HandlerPulse _handlerPulse;
        [Inject] private HandlerLoading _handlerLoading;

        private void Awake()
        {
            buttonFind.onClick.AddListener(Search);
        }

        public void ActivePanel(bool state)
        {
            searchPanel.SetActive(state);
        }

        public void Search()
        {
            var userName = inputName.GetText();
            if (userName == null) return;

            _handlerLoading.OpenLoadingScreen(true, HandlerLoading.Text.Finding);
            _handlerLoading.ActiveLoadingText(true);
            _handlerLoading.SetLoadingText("Search for " + userName);
            
            ActivePanel(false);
            
            UiFriends.DisableCatalogFocus();
            UiFriends.DisableEmptyListTip();
            UiFriends.ReferralWidget(UiFriends.Status.Null);
            
            inputName.Clear();

            FindAccountId(userName);
        }

        private void FindAccountId(string userName)
        {
            Debug.Log("Find Account Id for " + userName);

            PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { TitleDisplayName = userName },
            result =>
            {
                GetPlayerProfile(result.AccountInfo.PlayFabId);
            }, Error);
        }

        private void GetPlayerProfile(string playFabId)
        {
            Debug.Log("Get Player Profile by Id: " + playFabId);

            var constraints = new PlayerProfileViewConstraints();
            constraints.ShowLocations = true;
            constraints.ShowStatistics = true;
            constraints.ShowLastLogin = true;
            constraints.ShowDisplayName = true;

            PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
            {
                PlayFabId = playFabId,
                ProfileConstraints = constraints,
            },
            result =>
            {
                CreateUserSlot(result.PlayerProfile, playFabId);
            }, Error);
        }

        private void Error(PlayFabError error)
        {
            _handlerPulse.OpenTextNote(error.ErrorMessage);
            _handlerLoading.OpenLoadingScreen(false);
            _handlerLoading.ActiveLoadingText(false);
            Debug.LogError(error.GenerateErrorReport());
        }

        private async void CreateUserSlot(PlayerProfileModel profile, string friendPlayFabId)
        {
            ClearFactoryContainer();
            
            var userSlot = await CreateSlot(SlotName.UserSlotMenu);
                
            if (userSlot.TryGetComponent(out UiUserSlot userInfo))
            {
                userInfo.SetUserStatus(UiFriends.Status.External);
                userInfo.SetPhotonData(null);
                userInfo.SetPlayFabId(friendPlayFabId);
                userInfo.SetPlayFabProfile(profile);
                userInfo.SetInfoHandle(UserInfoHandle);
                
                if (GetUserInfo().payload.GetPlayFabId() == friendPlayFabId)
                {
                    userInfo.BlockUserButton();
                }
            }
                
            if (userSlot.TryGetComponent(out UiUserSlotPlayFab playFabInfo))
            {
                playFabInfo.SetProfileInfo(profile);
                playFabInfo.SetStatus(UiFriends.Status.External);
            }
            
            _handlerLoading.OpenLoadingScreen(false);
            _handlerLoading.ActiveLoadingText(false);
        }
    }
}
