using CodeStage.AntiCheat.ObscuredTypes;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;

namespace Playstel
{
    public class RoomInfo : ScriptableObject
    {
        public ObscuredString roomName;
        public ObscuredString sceneKey;
        public ObscuredString roomMusic;
        public ObscuredByte maxPlayers;

        public enum Setup
        {
            MaxPlayers, SceneKey, Music
        }

        public void SetRoomInfo(ItemInfo itemInfo)
        {
            var customData = itemInfo.GetUnsafeCustomData();

            roomName = itemInfo.itemName;

            sceneKey = DataHandler.GetUnsafeValue
                (customData, Setup.SceneKey.ToString());

            var maxPlayersString = DataHandler.GetUnsafeValue
                (customData, Setup.MaxPlayers.ToString());

            maxPlayers = byte.Parse(maxPlayersString);

            roomMusic = DataHandler.GetUnsafeValue
                (customData, Setup.Music.ToString());
        }

        public RoomOptions GetRoomOptions(bool isOpen = true, bool isVisible = true)
        {
            RoomOptions room = new RoomOptions();

            room.IsOpen = isOpen;
            room.MaxPlayers = maxPlayers;
            room.IsVisible = isVisible;
            room.PublishUserId = true;
            room.EmptyRoomTtl = 100;
            room.CleanupCacheOnLeave = true;
            room.DeleteNullProperties = false;

            room.CustomRoomPropertiesForLobby = new string[] { KeyStore.SCENE_KEY };
            room.CustomRoomProperties = SetRoomCustomProperties();

            return room;
        }

        private Hashtable SetRoomCustomProperties()
        {
            Hashtable roomHash = new Hashtable();
            roomHash.Add(KeyStore.SCENE_KEY, sceneKey.ToString());
            return roomHash;
        }
    }
}
