using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class CrateBoot : MonoBehaviourPun
    {
        public Type crateType;
        public Mesh crateGizmoMesh;
        
        private Crate _crate;
        public enum Type
        {
            Outdoor, Indoor
        }

        [Inject] private LocationInstaller _locationInstaller;

        private void Awake()
        {
            _crate = GetComponent<Crate>();
        }

        private void Start()
        {
            _crate.Stat.Install(crateType);
            BootCrate();
        }

        private async void BootCrate()
        {
            await UniTask.Delay(1000);
            
            if(!PhotonNetwork.IsMasterClient) return;

            var crateName = _crate.Handler.GetRandomCrateInstanceName();

            if (string.IsNullOrEmpty(crateName)) return;
            
            var crateItems = _crate.Handler.GetRandomCrateItemNames();
            
            _crate.Items.UpdateItems(crateItems.ToArray());
            
            photonView.RPC(nameof(RPC_CreateCrateInstance), RpcTarget.AllBuffered, crateName);
            
            _crate.Items.ActiveNewItemsTimer(true);
        }
        
        [PunRPC]
        private async void RPC_CreateCrateInstance(string crateName)
        {
            var instance = await CreateCrateInstance(crateName);
            
            _crate.SetInstance(instance);
            _crate.BindInstanceToTrigger();
        }

        private async UniTask<GameObject> CreateCrateInstance(string crateName)
        {
            return await _locationInstaller.LoadElement<GameObject>(crateName, transform);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            
            Gizmos.DrawMesh
                (crateGizmoMesh, 0, transform.position, transform.rotation, new Vector3(1,1,1));
        }
    }
}
