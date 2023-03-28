using System;
using System.IO;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using static Playstel.EnumStore;
using Random = UnityEngine.Random;

namespace Playstel
{
    public class BoosterInstanceBoot : MonoBehaviourPun
    {
        public Mesh gizmoMesh;
        private GameObject currentBooster;
        
        [Inject] private LocationInstaller _locationInstaller;
        [Inject] private CacheBoosters _cacheBoosters;

        private void Start()
        {
            Reboot();
        }

        private ObscuredInt rebootTime = 20000; 
        private async void Reboot()
        {
            if(!PhotonNetwork.IsMasterClient) return;

            await UniTask.Delay(rebootTime);

            if (!_cacheBoosters) return;
            
            var booster = _cacheBoosters.GetRandomBooster();
            
            if(booster == null) return;

            var boosterPosition = GetRandomBoosterTransform();

            photonView.RPC(nameof(RPC_CreateBoosterInstance), RpcTarget.AllBuffered, 
                booster.boosterName, boosterPosition);
        }

        private Vector3 GetRandomBoosterTransform()
        {
            return transform.position + SpawnSpread();
        }
        
        //private ObscuredString objectFolder = "Boosters";
        private async void RPC_CreateBoosterInstance(string boosterName, Vector3 boosterPosition)
        {
            currentBooster = await _locationInstaller.LoadElement<GameObject>
                (boosterName, transform);
            
            currentBooster.transform.position = boosterPosition;
            
            var notify = currentBooster.AddComponent<NotifyOnDestroyBoost>();
            notify.Destroyed += Reboot;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            
            Gizmos.DrawMesh
                (gizmoMesh, 0, transform.position, transform.rotation, new Vector3(1,1,1));
        }
        
        private Vector3 SpawnSpread()
        {
            return new Vector3(SpreadX(), 0, SpreadZ());
        }

        private const float spreadRange = 4f;
        private float SpreadX()
        {
            return Random.insideUnitSphere.x * spreadRange;
        }

        private float SpreadZ()
        {
            return Random.insideUnitSphere.z * spreadRange;
        }
    }
}