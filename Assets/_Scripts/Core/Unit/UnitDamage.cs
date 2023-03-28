using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UnitDamage : MonoBehaviour
    {
        private Unit _unit;

        [Inject] private CacheAudio _cacheAudio;
        
        private void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        private float _shakeFactor = 0.35f;
        public void SetDamage(int damageAmount, int bulletHostID, Vector3 hitPosition)
        {
            if (_unit.Death.IsDead()) return;
            
            _unit.Animation.Hit();

            if (!_unit.photonView.IsMine) return;

            _cacheAudio.Play(CacheAudio.MenuSound.OnDamage, true, 0.1f, true);

            if (_unit.Camera)
            {
                _unit.Camera.GetCameraObserver().Shake(_shakeFactor, 0.2f);
            }

            if (_unit.AreaBehaviour.GetCurrentArea() == UnitAreaBehaviour.Area.Safe)
            {
                return;
            }
            
            var newHealthValue = _unit.Health.GetUnitHealth() - damageAmount;

            _unit.Buffer.ChangeHealth(damageAmount);
            
            if (newHealthValue <= 0)
            {
                _unit.Callback.Death(bulletHostID, hitPosition); 
            }
        }

        private bool deadArea;
        public void DeadArea(bool state)
        {
            deadArea = state;
        }

        private float time;
        private ObscuredFloat hitTime = 0.5f;
        private ObscuredInt damagePerHit = 5;
        private void Update()
        {
            if(!deadArea) return;

            time += Time.deltaTime;

            if (time > hitTime)
            {
                time = 0;
                SetDamage(damagePerHit, 0, transform.position);
            }
        }
    }
}