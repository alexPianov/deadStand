using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class CrateItems : MonoBehaviourPun
    {
        public List<Item> crateItems = new ();
        public float cooldownTimer;
        
        [Inject] 
        private CacheItemInfo _cacheItemInfo;
        private Crate _crate;
        private bool _newItemsTimer;

        private void Awake()
        {
            _crate = GetComponent<Crate>();
        }

        public void ActiveNewItemsTimer(bool state)
        {
            _newItemsTimer = state;
        }

        private void Update()
        {
            if(!PhotonNetwork.IsMasterClient) return;
            
            if(!_newItemsTimer) return;

            if(crateItems.Count == 0)
            {
                cooldownTimer += Time.deltaTime;

                if (cooldownTimer >= _crate.Handler.GetRespawnCooldown())
                {
                    cooldownTimer = 0;
                    UpdateItems(_crate.Handler.GetRandomCrateItemNames().ToArray());
                }
            }
        }
        
        public void UpdateItems(List<Item> itemList)
        {
            List<string> items = new();
            
            foreach (var item in itemList)
            {
                items.Add(item.info.itemName);
            }

            var itemsStringArray = items.ToArray();
            photonView.RPC(nameof(RPC_UpdateItems), RpcTarget.AllBuffered, itemsStringArray as object);
        }

        public void UpdateItems(string[] items)
        {
            photonView.RPC(nameof(RPC_UpdateItems), RpcTarget.AllBuffered, items as object);
        }

        [PunRPC]
        private void RPC_UpdateItems(object itemsBoxed)
        { 
            crateItems = _cacheItemInfo.CreateItemList(
                (string[])itemsBoxed, 
                ItemInfo.Catalog.Backpack, 
                ItemInfo.Class.Loot);
        }

        public List<Item> Get()
        {
            return crateItems;
        }
    }
}