using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Playstel
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class UiElementDisableDropdownInRoom: MonoBehaviour
    {
        public void OnEnable()
        {
            GetComponent<TMP_Dropdown>().interactable = !PhotonNetwork.InRoom;
        }
    }
}
