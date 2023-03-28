using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    [RequireComponent(typeof(UiFriendsPhoton), typeof(UiUserReferralCode), typeof(UiUserInfoHandle))]
    public class UiFriends : UiFactory
    {
        public Button refreshButton;
        
        private UiFriendsPhoton _friendsPhoton;
        private UiFriendsEmptyListTip _friendsEmptyList;
        private UiUserReferralCode _uiUserReferralCode;
        private UiUserInfoHandle _uiUserInfoHandle;

        [Inject] private CacheUserFriends _cacheUserFriends;
        [Inject] private HandlerLoading _handlerLoading;

        private List<UiFriendsCatalogButton> _uiFriendsCatalogButtons = new();
        private List<UiFriendsCatalogCounter> _uiFriendsCatalogCounters = new();

        private void Awake()
        {
            _friendsPhoton = GetComponent<UiFriendsPhoton>();
            _friendsEmptyList = GetComponent<UiFriendsEmptyListTip>();
            _uiUserReferralCode = GetComponent<UiUserReferralCode>();
            _uiUserInfoHandle = GetComponent<UiUserInfoHandle>();
            
            _uiFriendsCatalogButtons = GetComponentsInChildren<UiFriendsCatalogButton>().ToList();
            _uiFriendsCatalogCounters = GetComponentsInChildren<UiFriendsCatalogCounter>().ToList();
            
            refreshButton.onClick.AddListener(Refresh);
        }

        public enum Status
        {
            Confirmed,
            Invited,
            Requested,
            Pending,
            External,
            Null
        }

        private Status lastStatus = Status.Null;

        public async void Refresh()
        {
            refreshButton.interactable = false;
            
            _handlerLoading.OpenLoadingPopup(true, HandlerLoading.Text.Updating);
            
            await _cacheUserFriends.UpdateFriendList();
            
            ShowUsers(lastStatus, true);
            
            _handlerLoading.OpenLoadingPopup(false);

            await UniTask.Delay(3000);
            
            if(refreshButton) refreshButton.interactable = true;
        }

        private void RefreshCatalogCounters()
        {
            foreach (var catalogCounter in _uiFriendsCatalogCounters)
            {
                catalogCounter.RefreshCounter();
            }
        }

        public void ShowUsers(Status status, bool skipLastStatusReturn = false)
        {
            var playFabFriends = _cacheUserFriends.GetPlayFabFriends(status);
            ShowUsers(status, playFabFriends, skipLastStatusReturn);
            RefreshCatalogCounters();
        }
        
        public async void ShowUsers(Status status, List<FriendInfo> playFabFriends, bool skipLastStatusReturn)
        {
            if(!skipLastStatusReturn && lastStatus == status) return;
            
            lastStatus = status;

            ClearFactoryContainer();
            ActiveCatalogButtonFocus(status);
            ReferralWidget(status);

            DisableEmptyListTip(); 
            
            if (playFabFriends == null || playFabFriends.Count == 0)
            {
                _friendsEmptyList.SetEmptyListTip(status); 
                return;
            }
            
            var photonFriends = await _friendsPhoton.GetPhotonFriends(playFabFriends);
            
            foreach (var playFabFriend in playFabFriends)
            {
                var userSlot = await CreateSlot(SlotName.UserSlotMenu);
                
                var photonFriend = GetPhotonFriend(photonFriends, playFabFriend);
                
                if (userSlot.TryGetComponent(out UiUserSlot userInfo))
                {
                    userInfo.SetUserStatus(status);
                    userInfo.SetPhotonData(photonFriend);
                    userInfo.SetPlayFabId(playFabFriend.FriendPlayFabId);
                    userInfo.SetPlayFabProfile(playFabFriend.Profile);
                    userInfo.SetInfoHandle(_uiUserInfoHandle);
                    
                    if (GetUserInfo().payload.GetPlayFabId() == playFabFriend.FriendPlayFabId)
                    {
                        userInfo.BlockUserButton();
                    }
                }
                
                if (userSlot.TryGetComponent(out UiUserSlotPlayFab playFabInfo))
                {
                    playFabInfo.SetProfileInfo(playFabFriend.Profile);
                    
                    if (status is not (Status.Confirmed and Status.Invited))
                    {
                        playFabInfo.SetStatus(status);
                    }
                }

                if (photonFriends == null)
                {
                    Debug.Log("Photon Friends is null");
                    continue;
                }
                
                if (userSlot.TryGetComponent(out UiUserSlotPhoton photonInfo))
                {
                    photonInfo.SetRealtimeInfo(photonFriend);

                    if (status is Status.Confirmed or Status.Invited)
                    {
                        photonInfo.SetStatus(photonFriend);
                    }
                }
            }
        }

        public void DisableEmptyListTip()
        {
            _friendsEmptyList.SetTipText(null);
        }

        private static Photon.Realtime.FriendInfo GetPhotonFriend
            (List<Photon.Realtime.FriendInfo> photonFriends, FriendInfo friendInfo)
        {
            if (photonFriends == null || photonFriends.Count == 0)
            {
                Debug.Log("Photon friends is null");
                return null;
            }
            
            return photonFriends
                .Find(item => item.UserId == friendInfo.FriendPlayFabId);
        }

        public void ActiveCatalogButtonFocus(Status status)
        {
            foreach (var catalogButton in _uiFriendsCatalogButtons)
            {
                catalogButton.SetFocus(catalogButton.currentStatus == status);
            }
        }

        public void DisableCatalogFocus()
        {
            foreach (var catalogButton in _uiFriendsCatalogButtons)
            {
                catalogButton.SetFocus(false);
            }
        }

        public void ReferralWidget(Status status)
        {
            _uiUserReferralCode.OpenReferralWidget(status == Status.Invited);
        }
    }
}
