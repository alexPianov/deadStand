using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class PlayerBootRoomAi : MonoBehaviourPun
    {
        public RoomPlayers RoomPlayers;
        public bool BotsIsCreated;
        
        [Inject] private LocationInstaller _locationInstaller;
        private void Awake()
        {
            _locationInstaller.BindFromInstance(this);
        }

        #region Send Player Data To Network

        public async void CreateBots()
        {
            Debug.Log("Create Bots");
            
            if(BotsIsCreated) return;
            
            for (int i = 0; i < RoomPlayers.MaxPlayersInTeam; i++)
            {
                CreateBot(UnitFraction.Fraction.Red);
                CreateBot(UnitFraction.Fraction.Blue);
                await UniTask.Delay(500);
            }

            BotsIsCreated = true;
        }

        public void CreateBot(UnitFraction.Fraction fractionId)
        {
            photonView.RPC(nameof(RPC_AddBot), RpcTarget.MasterClient, fractionId);
        }
        
        #endregion

        #region Create Unit Instance

        [PunRPC]
        private void RPC_AddBot(UnitFraction.Fraction fraction)
        {
            if (!FractionSpawner.Spawner)
            {
                Debug.LogError("Failed to find fraction spawner in the scene"); return;
            }
            
            var mineUnit = PhotonInstantiate();

            photonView.RPC(nameof(RPC_InstallSessionUnit), 
                RpcTarget.AllBuffered, 
                mineUnit.GetPhotonView().ViewID, fraction);
        }

        private ObscuredString objectFolder = "Prefabs";
        private ObscuredString objectName = "Bot";
        private GameObject PhotonInstantiate()
        {
            var mineUnit = PhotonNetwork.Instantiate
            (Path.Combine(objectFolder, objectName),
                transform.position + new Vector3(0,-10, 0), Quaternion.identity);
            
            mineUnit.transform.SetParent(transform);
            
            return mineUnit;
        }
        
        #endregion

        #region Install Player Data To Unit

        [PunRPC]
        private async void RPC_InstallSessionUnit(int unitViewId, UnitFraction.Fraction fraction)
        {
            var roomUnit = PhotonView.Find(unitViewId).GetComponent<Unit>();
            
            if (!roomUnit) { Debug.LogError("Failed to find room unit with PvId " + unitViewId); return; }

            var aiBoot = roomUnit.GetComponent<UnitAiBoot>();
            
            roomUnit.Ragdoll.EnableNavMeshAgent(false);
            roomUnit.Fraction.SetFractionNumber(fraction);
            roomUnit.Gizmo.SetPlayerGizmo();
            aiBoot.SetUnitNickName(GetRandomName());
            
            RoomPlayers.SetPlayer(fraction, roomUnit);
            
            await BindLevelCamera(roomUnit);
            await aiBoot.Install();

            roomUnit.gameObject.transform.position = FractionSpawner.Spawner
                .GetSpawn(fraction);
            
            roomUnit.Ragdoll.EnableNavMeshAgent(true);
        }

        private async UniTask BindLevelCamera(Unit unit)
        {
            await _locationInstaller.InstantiateComponent<UnitAiCamera>(unit.gameObject);
        }
        
        #endregion

        public string GetRandomName()
        {
            var nameType = Random.Range(1, 7);
            var nameClass = Random.Range(0, 1);
            var nameAdditive = Random.Range(0, 2);
            
            string nameFull = NVJOBNameGen.GiveAName(nameType);
            string nameSplit = nameFull.Split(" ")[nameClass];
            if (nameAdditive == 0) nameSplit += Random.Range(1, 100);
            
            return NVJOBNameGen.Uppercase(nameSplit);
        }
    }
}