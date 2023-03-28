using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

namespace Playstel
{
    [CreateAssetMenu(menuName = "Room/Join")]
    public class RoomJoin : ScriptableObject
    {
        public bool JoinExistingRoom(RoomInfo roomInfo)
        {
            Hashtable expectedCustomRoomProperties = new Hashtable 
            { 
                { KeyStore.SCENE_KEY, (string)roomInfo.sceneKey } 
            };

            return PhotonNetwork.JoinRandomRoom
                (expectedCustomRoomProperties, roomInfo.maxPlayers);
        }

    }
}
