using CodeStage.AntiCheat.ObscuredTypes;
using EventBusSystem;
using Photon.Pun;
using UnityEngine;

namespace Playstel
{
    public class UnitExperience : MonoBehaviourPun
    {
        public ObscuredInt maxExperience;
        public ObscuredInt currentExperience;
        public ObscuredInt currentLevel;

        private Unit _unit;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        public int GetLevel()
        {
            return currentLevel;
        }
        
        public void SetMaxExperience(int MaxExperience)
        {
            maxExperience = MaxExperience;
            UpdateMaxExperienceUi();
        }

        public void UpdateLevel(int value)
        {
            currentLevel = value;
        
            UpdateLevelUi();
        }

        public void UpdateExperience(int value)
        {
            currentExperience = value;

            if (currentExperience >= maxExperience)
            {
                currentExperience = currentExperience - maxExperience;
                Debug.Log("currentExperience: " + currentExperience + " / maxExperience: " + maxExperience);
                _unit.Buffer.ChangeLevel(1);
            }

            if (_unit.photonView.IsMine)
            {
                PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Experience, 
                    (int)currentExperience, _unit.photonView.Owner);
            }
            
            UpdateExperienceUi();
        }

        public void UpdateExperienceUi()
        {
            if(!_unit.photonView.IsMine) return;

            EventBus.RaiseEvent<IExperienceHandler>
                (h => h.HandleExperienceChange(currentExperience));
        }
        
        public void UpdateLevelUi()
        {
            if(!_unit.photonView.IsMine) return;

            EventBus.RaiseEvent<IExperienceHandler>
                (h => h.HandleLevelChange(currentLevel));
        }

        public void UpdateMaxExperienceUi()
        {
            if(!_unit.photonView.IsMine) return;

            EventBus.RaiseEvent<IExperienceHandler>
                (h => h.HandleMaxExperience(maxExperience));
        }

        private void NewLevelEffect()
        {
            
        }
    }
}
