using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    [CreateAssetMenu(menuName = "Room/Create")]
    public class RoomCreate : ScriptableObject
    {
        public bool CreateNewRoom(RoomOptions roomOptions, List<string> expectedFriends = null)
        {
            string[] friends = null;

            if (expectedFriends != null) friends = expectedFriends.ToArray();

            var createRoom = PhotonNetwork.CreateRoom(null, roomOptions,
                TypedLobby.Default, friends);
            
            Debug.Log("CreateRoom " + createRoom);

            return createRoom;
        }
    }
}
