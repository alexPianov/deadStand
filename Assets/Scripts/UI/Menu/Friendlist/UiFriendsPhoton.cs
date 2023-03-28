using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using PlayFab.ClientModels;
using UnityEngine;

namespace Playstel
{
    public class UiFriendsPhoton : MonoBehaviourPunCallbacks
    {
        public List<Photon.Realtime.FriendInfo> _photonFriends;

        public async UniTask<List<Photon.Realtime.FriendInfo>> GetPhotonFriends
            (List<FriendInfo> playFabFriends)
        {
            _photonFriends = null;
            
            if (playFabFriends == null || playFabFriends.Count == 0)
            {
                Debug.Log("PlayFab friends is null"); return null;
            }

            var friendNames = GetFriendNames(playFabFriends);

            var result = GenerateFirendsCallback(friendNames);

            if (!result)
            {
                Debug.LogError("Generate Firends Callback Error"); return null;
            }

            await UniTask.WaitUntil(() => _photonFriends != null);

            return _photonFriends;
        }

        private static List<string> GetFriendNames(List<FriendInfo> playFabFriends)
        {
            List<string> friendNames = new();

            foreach (var playFabFriend in playFabFriends)
            {
                friendNames.Add(playFabFriend.FriendPlayFabId);
            }

            return friendNames;
        }

        private static bool GenerateFirendsCallback(List<string> friendIds)
        {
            return PhotonNetwork.FindFriends(friendIds.ToArray());
        }
        
        public override void OnFriendListUpdate(List<Photon.Realtime.FriendInfo> photonFriends)
        {
            if (photonFriends == null)
            {
                Debug.Log("Photon Friends is null");
            }
            
            _photonFriends = photonFriends;
        }
    }
}
