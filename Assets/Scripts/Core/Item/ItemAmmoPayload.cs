using CodeStage.AntiCheat.ObscuredTypes;
using EventBusSystem;
using UnityEngine;

namespace Playstel
{
    public class ItemAmmoPayload : MonoBehaviour
    {
        public ObscuredInt _holders;
        public ObscuredInt _bullets;
        private Unit _unit;

        public void SetUnit(Unit unit)
        {
            _unit = unit;
        }
        
        public ObscuredInt GetHolders()
        {
            return _holders;
        }
        
        public ObscuredInt GetBullets()
        {
            return _bullets;
        }
        
        public void UpdateHolders(int value)
        {
            _holders = value;

            if (_holders < 0) _holders = 0; 

            UpdateBulletsUI();
        }
        
        public void UpdateBullets(int value)
        {
            _bullets = value;

            if (_bullets < 0) _bullets = 0;
            
            UpdateBulletsUI();
        }
        
        public void UpdateBulletsUI()
        {
            if(!_unit.photonView.IsMine || _unit.IsNPC) return;
            
            EventBus.RaiseEvent<IAmmoHandler>(h => h.HandleHolderChange
                (_bullets, _holders));
        }
    }
}