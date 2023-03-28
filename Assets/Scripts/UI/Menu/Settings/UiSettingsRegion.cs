using Photon.Pun;
using TMPro;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiSettingsRegion : MonoBehaviour
    {
        public TMP_Dropdown Dropdown;
        
        [Inject] private ConnectPhoton ConnectPhoton;
        [Inject] private CacheUserSettings _cacheUserSettings;
        [Inject] private HandlerPulse _handlerPulse;

        private const int regionCoefficient = 1;
        public void SetCurrentRegion(int regionNumber)
        {
            Dropdown.value = regionNumber - 1;
        }

        public async void PickRegion(TMP_Dropdown dropDown)
        {
            var pickedRegion = _cacheUserSettings.pickedRegionNumber;
            
            if(Dropdown.value + 1 == pickedRegion) return;
            if(dropDown.value + 1 == pickedRegion) return;

            var regionName = KeyStore.GetRegion(dropDown.value + regionCoefficient);
            
            if(string.IsNullOrEmpty(regionName)) return;
            
            _handlerPulse.OpenTextNote("Connect to " + regionName + " region");
            
            await ConnectPhoton.Connect(regionName, true);
            
            _handlerPulse.OpenTextNote("Connecting to " + regionName + " region is finish");
        }
    }
}
