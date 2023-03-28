using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiUserInfoHandle : MonoBehaviour
    {
        [Inject] private CacheUserFriends _cacheUserFriends;
        [Inject] private UIElementContainer _elementsContainer;
        [Inject] private HandlerLoading _handlerLoading;
        [Inject] private HandlerPulse _handlerPulse;
        [Inject] private ConnectRoom _connectRoom;
        [Inject] private CacheRatingList _cacheRatingList;
        [Inject] private CacheAudio _cacheAudio;

        public MenuType currentMenuType;
        
        
        public enum MenuType
        {
            Friendlist, Leaderboard
        }

        private UiFriends _uiFriends;
        private UiUserSlot _currentSlot;
        private UiUserInfoPanel _uiUserInfoPanel;
        
        public enum Function
        {
            Join, SeeCharacter, Subscribe, ConfirmSubscribe, Unsubscribe            
        }
        
        private void Awake()
        {
            _uiFriends = GetComponent<UiFriends>();
            _uiUserInfoPanel = GetComponent<UiUserInfoPanel>();
        }

        public void OpenUserInfo(UiUserSlot currentUserSlot)
        {
            _currentSlot = currentUserSlot;
            
            _uiUserInfoPanel.ActivePanel(true);
            _uiUserInfoPanel.SetUserName(currentUserSlot.PlayFabProfile.DisplayName);
            _uiUserInfoPanel.SetAvailableFunctionButtons(currentUserSlot);
        }

        public void Join()
        {
            Debug.Log("Join: " + _currentSlot.GetPhotonFriendInfo().Room);
            
            _cacheRatingList.ClearLeaderboardCache();
            _connectRoom.JoinTargetRoom(_currentSlot.GetPhotonFriendInfo().Room);
        }

        public void SeeCharacter()
        {
            var playerId = _currentSlot.PlayFabId;

            _elementsContainer.SetClipboardString
                (UIElementContainer.ClipboardStringType.FriendId, playerId);

            StatisticExternalScreen();
        }

        private void StatisticExternalScreen()
        {
            if (currentMenuType == MenuType.Friendlist)
            {
                LoadScreen(UIElementLoad.Elements.StatisticsExternalFriendlist);
            }

            if (currentMenuType == MenuType.Leaderboard)
            {
                LoadScreen(UIElementLoad.Elements.StatisticsExternalLeaderboard);
            }
        }

        private async UniTask LoadScreen(UIElementLoad.Elements element)
        {
            _handlerLoading.ActiveLoadingText(true);
            _handlerLoading.SetLoadingText("Load External User");
            
            await GetComponent<UIElementLoadByType>().Load(element, UIElementContainer.Type.Screen);
            
            _handlerLoading.SetLoadingText(null);
            _handlerLoading.ActiveLoadingText(false);
        }

        public async void Subscribe()
        {
            _handlerLoading.OpenLoadingScreen(true, HandlerLoading.Text.Subscribe);
            _handlerLoading.ActiveLoadingText(true);
            _handlerLoading.SetLoadingText("Sending request to " + _currentSlot.PlayFabProfile.DisplayName);
            
            await _cacheUserFriends.SubscribeToUser(_currentSlot.PlayFabId);
            _handlerPulse.OpenTextNote(_currentSlot.PlayFabProfile.DisplayName + " is got your request");
            
            _uiUserInfoPanel.ActivePanel(false);
            if(_uiFriends)_uiFriends.ShowUsers(UiFriends.Status.Confirmed, true);
            _cacheAudio.Play(CacheAudio.MenuSound.OnSwitchUi);
            
            _handlerLoading.SetLoadingText(null);
            _handlerLoading.ActiveLoadingText(false);
            _handlerLoading.OpenLoadingScreen(false);
        }

        public async void ConfirmSubscribe()
        {
            _handlerLoading.OpenLoadingScreen(true, HandlerLoading.Text.Confirm);
            _handlerLoading.ActiveLoadingText(true);
            _handlerLoading.SetLoadingText("Adding " + _currentSlot.PlayFabProfile.DisplayName + " to friendlist");
            
            await _cacheUserFriends.ConfirmSubscribe(_currentSlot.PlayFabId);
            _handlerPulse.OpenTextNote(_currentSlot.PlayFabProfile.DisplayName + " is added to your friends");
            _uiUserInfoPanel.ActivePanel(false);
            if(_uiFriends) _uiFriends.ShowUsers(UiFriends.Status.Confirmed, true);
            _cacheAudio.Play(CacheAudio.MenuSound.OnSwitchUi);
            
            _handlerLoading.SetLoadingText(null);
            _handlerLoading.ActiveLoadingText(false);
            _handlerLoading.OpenLoadingScreen(false);
        }

        public async void Unsubscribe()
        {
            _handlerLoading.OpenLoadingScreen(true, HandlerLoading.Text.Unsubscribe);
            _handlerLoading.ActiveLoadingText(true);
            _handlerLoading.SetLoadingText("Removing " + _currentSlot.PlayFabProfile.DisplayName + " from friendlist");
            
            await _cacheUserFriends.Unsubscribe(_currentSlot.PlayFabId);
            _handlerPulse.OpenTextNote(_currentSlot.PlayFabProfile.DisplayName + " is removed from friends");

            _uiUserInfoPanel.ActivePanel(false);
            if(_uiFriends) _uiFriends.ShowUsers(UiFriends.Status.Confirmed, true);
            _cacheAudio.Play(CacheAudio.MenuSound.OnSwitchUi);
            
            _handlerLoading.SetLoadingText(null);
            _handlerLoading.ActiveLoadingText(false);
            _handlerLoading.OpenLoadingScreen(false);
        }
    }
}
