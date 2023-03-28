using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Photon.Pun;

namespace Playstel
{
    public class UnitDeathScreen : MonoBehaviourPun
    {
        private CacheAudio _cacheAudio;
        private Unit _unit;

        private void Start()
        {
            _unit = GetComponent<Unit>();
        }

        public async void OpenGameOverScreen()
        {
            if(!photonView.IsMine) return;
            if (_unit.IsNPC) return;

            await GetComponent<UIElementLoadByType>()
                .Load(UIElementLoad.Elements.RoundResult, UIElementContainer.Type.Screen);
        }
        
        private ObscuredInt _lastKillerViewId;
        public async UniTask OpenKillerScreen(int killerId)
        {
            if(!photonView.IsMine) return;
            if (_unit.IsNPC) return;
            
            _lastKillerViewId = killerId;
            var killer = PhotonView.Find(_lastKillerViewId).gameObject.transform;
            _unit.Camera.GetCameraObserver().Follow(killer);
            _unit.Camera.GetCameraObserver().SetMaxZoom(true);
            
            // await GetComponent<UIElementLoadByType>()
            //     .Load(UIElementLoad.Elements.KillerScreen, UIElementContainer.Type.Screen);
        }

        public Unit GetLastKillerUnit()
        {
            return PhotonView.Find(_lastKillerViewId).GetComponent<Unit>();
        }
    }
}