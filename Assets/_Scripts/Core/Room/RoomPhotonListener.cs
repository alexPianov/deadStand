using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using Playstel.Bootstrap;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class RoomPhotonListener : MonoBehaviourPunCallbacks
    {
        [Inject] private BootstrapInstaller _bootstrapInstaller;
        [Inject] private HandlerNetworkError _networkError;

        public async override void OnLeftRoom()
        {
            Debug.Log("On Left Room");
            
            PhotonHandler.ClearNetworkCustomProperties(PhotonNetwork.LocalPlayer);
            PhotonNetwork.OpRemoveCompleteCacheOfPlayer(PhotonNetwork.LocalPlayer.ActorNumber);

            base.OnLeftRoom();

            await UniTask.Delay(1000);
            
            if (PhotonNetwork.IsConnectedAndReady)
            {
                _bootstrapInstaller.DataBoot(false);
            }
            else
            {
                _networkError.ActivePanel(true);
            }
        }
    }
}
