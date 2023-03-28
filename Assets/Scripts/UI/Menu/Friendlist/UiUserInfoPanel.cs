using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiUserInfoPanel : MonoBehaviour
    {
        public GameObject infoPanel;
        public Transform buttonsLayout;
        public TextMeshProUGUI playerName;

        private List<UiUserInfoButton> _infoButtons = new ();

        [Inject] private CacheUserFriends _cacheUserFriends;

        private void Awake()
        {
            InitializeButtons();
        }

        private void InitializeButtons()
        {
            _infoButtons = buttonsLayout.GetComponentsInChildren<UiUserInfoButton>().ToList();
        }

        public void SetUserName(string nickName)
        {
            playerName.text = nickName;
        }

        public void SetAvailableFunctionButtons(UiUserSlot userSlot)
        {
            DisableFunctionButtons();
            
            EnableFunctionButton(UiUserInfoHandle.Function.SeeCharacter);

            if (userSlot.Status == UiFriends.Status.Confirmed || 
                userSlot.Status == UiFriends.Status.Invited)
            {
                if (userSlot.PhotonFriendInfo != null)
                {
                    if (userSlot.PhotonFriendInfo.IsInRoom)
                    {
                        EnableFunctionButton(UiUserInfoHandle.Function.Join);
                    }
                    else
                    {
                        Debug.Log("Friend is not in the room");
                    }
                }
                else
                {
                    Debug.Log("Photon Friend Info is null");
                }
                
                EnableFunctionButton(UiUserInfoHandle.Function.Unsubscribe);
            }

            if (userSlot.Status == UiFriends.Status.Pending)
            {
                EnableFunctionButton(UiUserInfoHandle.Function.ConfirmSubscribe);
                EnableFunctionButton(UiUserInfoHandle.Function.Unsubscribe);
            }

            if (userSlot.Status == UiFriends.Status.Requested)
            {
                EnableFunctionButton(UiUserInfoHandle.Function.Unsubscribe);
            }

            if (userSlot.Status == UiFriends.Status.External)
            {
                if(UserInFriends(userSlot.PlayFabId)) return;
                
                EnableFunctionButton(UiUserInfoHandle.Function.Subscribe);
            }
        }

        private bool UserInFriends(string playFabId)
        {
            var friendList = _cacheUserFriends.GetPlayFabFriends();
            
            var friend = friendList.Find
                (item => item.FriendPlayFabId == playFabId);
            
            return friend != null;
        }

        private void EnableFunctionButton(UiUserInfoHandle.Function function)
        {
            foreach (var button in _infoButtons)
            {
                if (button.currentFunction == function)
                    button.gameObject.SetActive(true);
            }
        }
        
        private void DisableFunctionButtons()
        {
            foreach (var button in _infoButtons)
                button.gameObject.SetActive(false);
        }

        public void ActivePanel(bool state)
        {
            infoPanel.SetActive(state);
        }
    }
}
