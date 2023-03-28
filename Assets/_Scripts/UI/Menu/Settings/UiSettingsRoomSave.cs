using Photon.Pun;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiSettingsRoomSave : MonoBehaviour
    {
        private Unit _unit;
        
        [Inject]
        private void GetUnit(Unit unit)
        {
            _unit = unit;
        }
        
        public void OnDisable()
        {
            if (PhotonNetwork.InRoom)
            {
                _unit.Camera.UpdateCameraSettings();
            }
        }
    }
}