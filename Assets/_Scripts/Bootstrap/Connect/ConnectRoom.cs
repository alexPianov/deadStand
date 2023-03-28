using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Playstel
{
    public class ConnectRoom : MonoBehaviourPunCallbacks
    {
        [Header("Setup")]
        public RoomJoin roomJoin;
        public RoomCreate roomCreate;
        public bool sceneInBuild;

        [Header("Room")]
        public List<RoomInfo> rooms = new ();
        public RoomInfo pickedRoom;

        [Header("Refs")] 
        public HandlerPulse HandlerPulse;
        public HandlerLoading HandlerLoading;
        public UserPayload UserPayload;

        public async UniTask Install(CacheItemInfo _cacheItemInfo)
        {
            var items = _cacheItemInfo
                .GetItemInfoList(ItemInfo.Catalog.Setup, ItemInfo.Class.Room);

            foreach (var item in items)
            {
                var room = ScriptableObject.CreateInstance<RoomInfo>();
                room.SetRoomInfo(item);
                rooms.Add(room);
            }

            SetPickedRoom(rooms[0]);
        }

        public void SetPickedRoom(RoomInfo room)
        {
            pickedRoom = room;
        }

        private bool NetworkPass()
        {
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                HandlerPulse.OpenTextNote("Connection is not ready. Try againg");
                return false;
            }
            
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
                HandlerPulse.OpenTextNote("Disconnect from recent room. Try againg");
                return false;
            }

            if (string.IsNullOrEmpty(UserPayload.GetTitleDisplayName()) || 
                string.IsNullOrWhiteSpace(UserPayload.GetTitleDisplayName()))
            {
                HandlerPulse.OpenTextNote("Name your character before playing");
                return false;
            }
            
            return true;
        }

        public void JoinTargetRoom(string roomName)
        {
            if(!NetworkPass()) return;
            
            HandlerLoading.OpenLoadingScreen(true, HandlerLoading.Text.Join);
            
            HandlerLoading.ActiveLoadingText(true);
            HandlerLoading.SetLoadingText("Join Friend Room");
            
            var result = PhotonNetwork.JoinRoom(roomName);
            
            if (!result)
            {
                HandlerPulse.OpenTextNote("Join room failed. Creating new room");
            }
            
            Debug.Log("ConnectFriendRoom: " + result);
        }

        public void ConnectPickedRoom()
        {
            if(!NetworkPass()) return;
            
            HandlerLoading.OpenLoadingScreen(true);
            
            HandlerLoading.ActiveLoadingText(true);
            HandlerLoading.SetLoadingText("Join Existing Room");
            
            var result = roomJoin.JoinExistingRoom(pickedRoom);
            
            Debug.Log("ConnectPickedRoom: " + result);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            HandlerLoading.SetLoadingText("Create New Room");
            Debug.Log("OnJoinRandomFailed: " + message);
            roomCreate.CreateNewRoom(pickedRoom.GetRoomOptions());
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            HandlerLoading.SetLoadingText("Create Room Failed");
            Debug.LogError("OnCreateRoomFailed: " + message + " | Return Code: " + returnCode);
            ConnectPickedRoom();
        }

        public override void OnCreatedRoom()
        {
            HandlerLoading.SetLoadingText("Room Is Created");
            Debug.Log("OnCreatedRoom");
        }

        public override void OnJoinedRoom()
        {
            HandlerLoading.SetLoadingText("Joined to Room");
            
            Debug.Log("OnJoinedRoom");
            LoadScene(sceneInBuild);
        }

        public async void LoadScene(bool sceneInBuild = false)
        {
            HandlerLoading.SetLoadingText("Load Game Scene");

            PhotonNetwork.CurrentRoom.CustomProperties
                .TryGetValue(KeyStore.SCENE_KEY, out object index);
            
            var scene = (string)index;

            if (sceneInBuild)
            {
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.LoadLevel(scene);
            }
            else
            {
                PhotonNetwork.AutomaticallySyncScene = false;
                PhotonNetwork.PauseNetworkWhileSceneLoading(scene);
                await Addressables.LoadSceneAsync(scene);
            }
        }
    }
}
