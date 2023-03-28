using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using System.Collections.Generic;
using EventBusSystem;
using Photon.Pun;
using Playstel.UI;
using UnityEngine;

namespace Playstel
{
    public class UnitSprint : MonoBehaviour
    {
        [Header("Mode")]
        public ObscuredBool sprint;

        [Header("Impacts")]
        public ObscuredFloat currentEndurance;
        
        private ObscuredInt _maxEndurance;
        private ObscuredInt defaultPenalty = 2;
        
        private Unit _unit;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            enabled = PhotonNetwork.InRoom;
        }

        public void SetMaxEndurance(int MaxEndurance)
        {
            _maxEndurance = MaxEndurance;
            UpdateUiMaxEndurance();
        }

        public void UpdateEndurance(int value)
        {
            currentEndurance = value;
            UpdateUiEndurance();
        }

        private ObscuredBool sprintPause;
        public void Sprint(bool state)
        {
            if (currentEndurance <= 0 || !_unit.Move.isRunning || sprintPause)
            {
                sprint = false; return;
            }

            sprint = state;
        }

        private void Update()
        {
            if (sprint)
            {
                currentEndurance -= Time.deltaTime * GetSprintPenalty();

                UpdateUiEndurance();

                if (currentEndurance <= 0) sprint = false; 
            }
            else
            {
                if (currentEndurance < _maxEndurance)
                {
                    currentEndurance += Time.deltaTime / GetSprintPenalty();
                    UpdateUiEndurance();
                }
            }
        }

        private float GetSprintPenalty()
        {
            if (_unit.HandleItems.currentItemStat == null) 
                return defaultPenalty;
            
            return _unit.HandleItems.currentItemStat.weight / 1.5f; // min: 1.20; max: 3.00
        }

        public void UpdateUi()
        {
            UpdateUiMaxEndurance();
            UpdateUiEndurance();
        }
        
        private void UpdateUiMaxEndurance()
        {
            if(!_unit.photonView.IsMine) return;
            
            EventBus.RaiseEvent<ISprintHandler>
                (h => h.HandleSprintMaxValue(_maxEndurance));
        }
        
        private void UpdateUiEndurance()
        {
            if(!_unit.photonView.IsMine) return;

            EventBus.RaiseEvent<ISprintHandler>
                (h => h.HandleSprintChange(currentEndurance));
        }
    }
}
