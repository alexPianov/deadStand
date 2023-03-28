using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Playstel
{
    public class NpcCharacterBootIceman : MonoBehaviourPun
    {
        public List<GameObject> places = new ();
        
        private void Start()
        {
            OpenRandomPlace();
        }

        private void OpenRandomPlace()
        {
            if(!PhotonNetwork.IsMasterClient) return;

            var placeNumber = Random.Range(0, places.Count);
            photonView.RPC(nameof(RPC_OpenPlace), RpcTarget.AllBuffered, placeNumber);
        }

        [PunRPC]
        private void RPC_OpenPlace(int placeNumber)
        {
            DisablePlaces();
            
            for (int i = 0; i < places.Count; i++)
            {
                if (i == placeNumber)
                {
                    places[i].SetActive(true);
                    break;
                }
            }
        }
        
        private void DisablePlaces()
        {
            for (int i = 0; i < places.Count; i++)
            {
                places[i].SetActive(false);
            }
        }
    }
}