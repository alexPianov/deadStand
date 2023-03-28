using System.IO;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using ShaderCrew.SeeThroughShader;
using UnityEngine;
using Zenject;

namespace Playstel
{
    [RequireComponent(typeof(UIElementLoadOnBoot), typeof(PlayerBootHash))]
    public class PlayerBootRoom : MonoBehaviourPun
    {
        public UIElementLoadOnBoot playerUi;
        public RoomSoundtrack RoomSoundtrack;
        public RoomPlayers RoomPlayers;
        public PlayerBootRoomAi Ai;
        
        [Inject] private LocationInstaller _locationInstaller;
        [Inject] private HandlerLoading _handlerLoading;
        [Inject] private CacheAudio _cacheAudio;

        private void Awake()
        {
            _locationInstaller.BindFromInstance(this);
        }

        #region Send Player Data To Network

        public async UniTask CreatePlayer(int fractionId)
        {
            _handlerLoading.OpenLoadingScreen(true, HandlerLoading.Text.Creating);
            _handlerLoading.ActiveLoadingText(true);
            _handlerLoading.SetLoadingText("Create Network Unit");

            await GetComponent<PlayerBootHash>().InstallPlayerNetworkCustomProperties(fractionId);

            photonView.RPC(nameof(RPC_AddPlayer), RpcTarget.MasterClient);
        }
        
        [PunRPC]
        private void RPC_AddPlayer(PhotonMessageInfo info)
        {
            if(info.Sender.IsMasterClient) Ai.CreateBots();
            photonView.RPC(nameof(RPC_CreateSessionUnit), info.Sender);
        }
        
        #endregion

        #region Create Unit Instance

        [PunRPC]
        private async void RPC_CreateSessionUnit()
        {
            _handlerLoading.SetLoadingText("Create Player Unit");
            
            if (!FractionSpawner.Spawner)
            {
                Debug.LogError("Failed to find fraction spawner in the scene"); return;
            }
            
            var mineUnit = PhotonInstantiate();

            if (mineUnit.TryGetComponent(out Unit unit))
            {
                if(!unit.photonView.IsMine) return;

                _locationInstaller.BindFromInstance(unit);
                
                BindWallShader(mineUnit);
            
                await BindPlayerUI();

                photonView.RPC(nameof(RPC_InstallSessionUnit), 
                    RpcTarget.AllBuffered, 
                    mineUnit.GetPhotonView().ViewID);
            }
        }

        private ObscuredString objectFolder = "Prefabs";
        private ObscuredString objectName = "Player";
        private GameObject PhotonInstantiate()
        {
            var mineUnit = PhotonNetwork.Instantiate(Path.Combine(objectFolder, objectName),
                transform.position + new Vector3(0,-10, 0), Quaternion.identity);
            
            mineUnit.transform.SetParent(transform);
            
            return mineUnit;
        }

        private void BindWallShader(GameObject mineUnit)
        {
            if (GetComponent<PlayersPositionManager>())
            {
                GetComponent<PlayersPositionManager>().playableCharacters.Add(mineUnit);
                mineUnit.AddComponent<PrefabInstance>();
            }
        }

        private async UniTask BindPlayerUI()
        {
            if (playerUi)
            {
                await playerUi
                    .Load(UIElementLoad.Elements.GUI, UIElementContainer.Type.Screen);
                
                await playerUi
                    .Load(UIElementLoad.Elements.AimThrow, UIElementContainer.Type.Screen, true);
            }
        }

        #endregion

        #region Install Player Data To Unit

        [PunRPC]
        private async void RPC_InstallSessionUnit(int unitViewId)
        {
            var roomUnit = PhotonView.Find(unitViewId).GetComponent<Unit>();
            
            if (!roomUnit) { Debug.LogError("Failed to find room unit with PvId " + unitViewId); return; }

            if (roomUnit.photonView.IsMine) _handlerLoading.SetLoadingText("Build Unit Appearance");

            await UniTask.WaitUntil(() => roomUnit.Boot);
            
            roomUnit.Ragdoll.EnableNavMeshAgent(false);
            roomUnit.Fraction.SetFractionNumber();
            roomUnit.Gizmo.SetPlayerGizmo();
            roomUnit.Boot.SetUnitNickName();
            
            await BindLevelCamera(roomUnit);
            await roomUnit.Boot.Install();

            roomUnit.gameObject.transform.position = FractionSpawner.Spawner
                .GetSpawn(roomUnit.Fraction.currentFraction);
            
            RoomPlayers.SetPlayer(roomUnit.Fraction.currentFraction, roomUnit);
            
            roomUnit.Ragdoll.EnableNavMeshAgent(true);
            
            if (roomUnit.photonView.IsMine)
            {
                RoomSoundtrack.PlayRoundSoundtrack();
                roomUnit.gameObject.AddComponent<AudioListener>();
                
                _cacheAudio.ActiveAudioListener(false);
                
                _handlerLoading.OpenLoadingScreen(false);
                _handlerLoading.ActiveLoadingText(false);
                _handlerLoading.SetLoadingText(null);
            }
        }

        private async UniTask BindLevelCamera(Unit unit)
        {
            await _locationInstaller.InstantiateComponent<UnitCamera>(unit.gameObject);
        }
        
        #endregion

    }
}