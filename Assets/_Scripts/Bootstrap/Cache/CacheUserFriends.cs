using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using PlayFab.ClientModels;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon.StructWrapping;

namespace Playstel
{
	public class CacheUserFriends : MonoBehaviour
	{
		[Header("Setup")]
		public UserPayload userPayload;

		public List<FriendInfo> _playFabFriends = new();

		public HandlerPulse HanlderPulse;

        public async UniTask Install()
		{
			await UpdateFriendList();
		}

        public async UniTask UpdateFriendList()
        {
	        _playFabFriends = await PlayFabHandler.GetFriendList();
        }

        public List<FriendInfo> GetPlayFabFriends()
        {
	        return _playFabFriends;
        }
        
        public List<FriendInfo> GetPlayFabFriends(UiFriends.Status status)
        {
	        if (_playFabFriends == null || _playFabFriends.Count == 0)
	        {
		        Debug.Log("PlayFab friends is null");
		        return null;
	        }
            
	        return _playFabFriends.FindAll(item => item.Tags.Contains(status.ToString()));
        }

        #region Subscribe

        public async UniTask SubscribeToUser(string friendPlayFabId)
        {
	        var result = await PlayFabHandler.AddFriend(friendPlayFabId);

	        if (result == null)
	        {
		        HanlderPulse.OpenTextNote("User Not Found");
	        }
	        else
	        {
		        await AddUserToExpectedFriendAsRequester(friendPlayFabId);
	        }
        }

        private async UniTask AddUserToExpectedFriendAsRequester(string friendPlayFabId)
        {
	        var result = await PlayFabHandler.GetFriendList();

	        if (result == null)
	        {
		        HanlderPulse.OpenTextNote("Friendlist is not found");
	        }
	        else
	        {
		        await AddFriendById(friendPlayFabId, userPayload.GetPlayFabId());
		        
		        await UpdateUsersTags(friendPlayFabId);
		        
		        UpdateFreindTags(friendPlayFabId);
		        
		        UpdateFriendList();
	        }
        }
        private async Task UpdateFreindTags(string friendPlayFabId)
        {
	        SetExpectedFriendTagForUser(friendPlayFabId,
		        UiFriends.Status.Pending.ToString(), userPayload.GetPlayFabId());
        }

        private async Task UpdateUsersTags(string friendPlayFabId)
        {
	        await SetExpectedFriendTagForUser(userPayload.GetPlayFabId(),
		        UiFriends.Status.Requested.ToString(), friendPlayFabId);
        }

        private async UniTask AddFriendById(string userId, string friendId)
        {
	        var args = new
	        {
		        PlayFabId = userId, FriendId = friendId
	        };

	        var result = await PlayFabHandler.ExecuteCloudScript
		        (PlayFabHandler.Function.AddFriend, args);

	        if (result == null)
	        {
		        HanlderPulse.OpenTextNote("User Not Found");
	        }
        }

        #endregion

        #region Subscribe Confirm

        public async UniTask ConfirmSubscribe(string friendPlayFabId)
        {
	        await SetExpectedFriendTagForUser
	        (userPayload.GetPlayFabId(), UiFriends.Status.Confirmed.ToString(),
		        friendPlayFabId);
			
	        SetExpectedFriendTagForUser
	        (friendPlayFabId, UiFriends.Status.Confirmed.ToString(), 
		        userPayload.GetPlayFabId());
	        
	        await UpdateFriendList();
        }

        #endregion

        #region Remove

        public async UniTask Unsubscribe(string friendPlayFabId)
        {
	        var result = await PlayFabHandler.RemoveFriend(friendPlayFabId);

	        if (result == null)
	        {
		        HanlderPulse.OpenTextNote("User Not Found");
		        return;
	        }
	        
	        Debug.Log("Friend removed: " + friendPlayFabId);
	        RejectFriendshipRequest(friendPlayFabId, userPayload.GetPlayFabId());
	        await UpdateFriendList();
        }

        private async UniTask RejectFriendshipRequest(string userId, string friendId)
        {
	        var args = new
	        {
		        PlayFabId = userId, FriendId = friendId
	        };
			
	        var result = await PlayFabHandler
		        .ExecuteCloudScript(PlayFabHandler.Function.RemoveFriend, args);

	        if (result == null)
	        {
		        HanlderPulse.OpenTextNote("User Not Found");
	        }
	        else
	        {
		        Debug.Log("Friend " + friendId + " is removed for " + userId);
	        }
        }

        #endregion

		#region Set Tag

		private async UniTask SetExpectedFriendTagForUser(string userId, 
			string friendTags, string friendId)
		{
			var args = new
			{
				PlayFabId = userId, FriendId = friendId, FriendTags = friendTags
			};
			
			var result = await PlayFabHandler
				.ExecuteCloudScript(PlayFabHandler.Function.SetFriendTags, args);
			
			if(result == null) 
			{
				HanlderPulse.OpenTextNote("User Not Found");
			}
			else
			{
				Debug.Log("Friend " + friendId + " tag is set for " + userId);
			}
		}

		#endregion
	}
}
