using Cysharp.Threading.Tasks;
using EventBusSystem;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UnitAreaBehaviour : MonoBehaviour
    {
        public Area currentArea = Area.Unsafe;
        
        private Unit _unit;
        [Inject] private CacheAudio _cacheAudio;

        private void Start()
        {
            _unit = GetComponent<Unit>();
        }

        public Area GetCurrentArea()
        {
            return currentArea;
        }
        
        public enum Area
        {
            Safe, Unsafe, Dead 
        }

        public async void SetAreaBehaviour(Area area, string areaName = null, bool sound = true)
        {
            if(currentArea == area) return;
            
            await UniTask.WaitUntil(() => _unit.Camera);
            await UniTask.WaitUntil(() => _unit.Camera.GetCameraObserver());
            
            currentArea = area;

            switch (currentArea)
            {
                case Area.Safe:
                    AreaSafe(areaName);
                    break;
                case Area.Unsafe:
                    AreaUnsafe(areaName);
                    break;
                case Area.Dead:
                    AreaDead(areaName);
                    break;
                default:
                    return;
            }
            
            if(sound) _cacheAudio.Play(CacheAudio.MenuSound.OnSplash);
        }

        private void AreaSafe(string areaName)
        {
            if (!string.IsNullOrEmpty(areaName))
            {
                Announce("You are in " + areaName + " area");
            }
            
            AimingPermission(false);
            
            _unit.Aim.Attack(false);
            _unit.Aim.Aiming(false);

            _unit.Damage.DeadArea(false);
            
            _unit.Camera.GetCameraObserver().SetMaxZoom(true);
        }

        private async void AreaUnsafe(string areaName)
        {
            if (!string.IsNullOrEmpty(areaName))
            {
                Announce("You are leaving " + areaName + " area");
            }
            
            AimingPermission(true);

            _unit.Damage.DeadArea(false);
            
            await UniTask.WaitUntil(() => _unit.Camera.GetCameraObserver());
            
            _unit.Camera.GetCameraObserver().SetMaxZoom(false);
        }

        private void AreaDead(string areaName)
        {
            if (!string.IsNullOrEmpty(areaName))
            {
                Announce("Warning! You are in " + areaName + " area");
            }
            
            AimingPermission(false);
            
            _unit.Aim.Attack(false);
            _unit.Aim.Aiming(false);
            
            _unit.Damage.DeadArea(true);
            _unit.Camera.GetCameraObserver().SetMaxZoom(false);
        }

        private bool currentAimPermission = true;
        private void AimingPermission(bool state)
        {
            currentAimPermission = state;
            
            EventBus.RaiseEvent<IAimingPermissionHandler>(h =>
                h.HandlerAimingPermission(state));
        }

        public void RefreshCurrentAimingPermission()
        {
            EventBus.RaiseEvent<IAimingPermissionHandler>(h =>
                h.HandlerAimingPermission(currentAimPermission));
        }

        private static void Announce(string text)
        {
            EventBus.RaiseEvent<IAnnounceHandler>(h =>
                h.HandleValue(text, true));
        }
    }
}