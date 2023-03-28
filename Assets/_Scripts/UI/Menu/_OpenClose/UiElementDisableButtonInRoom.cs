using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Playstel
{
    [RequireComponent(typeof(Button))]
    public class UiElementDisableButtonInRoom: MonoBehaviour
    {
        public void OnEnable()
        {
            GetComponent<Button>().interactable = !PhotonNetwork.InRoom;
        }
    }
}
