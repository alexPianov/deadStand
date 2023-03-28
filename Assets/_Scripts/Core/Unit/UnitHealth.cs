using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Playstel.UI;
using UnityEngine;

namespace Playstel
{
    public class UnitHealth : MonoBehaviour
    {
        public GuiUnitHealth GuiUnitHealth;
        
        public ObscuredInt _currentHealth;
        public ObscuredInt _maxHealth;

        private Unit _unit;
        private void Start()
        {
            _unit = GetComponent<Unit>();
        }

        public void ActiveHealthUi(bool state)
        {
            GuiUnitHealth.HandleHealthActive(state);
        }
        
        public void SetMaxHealth(int MaxHealth)
        {
            _maxHealth = MaxHealth;
            GuiUnitHealth.HandleHealthMaxValue(_maxHealth);
        }

        public int GetMaxHealth()
        {
            return _maxHealth;
        }

        public int GetUnitHealth()
        {
            return _currentHealth;
        }

        public void UpdateHealth(int value, bool updateUi)
        {
            _currentHealth = value;
            
            if (_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
            }
            
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
            }
            
            if(updateUi) UpdateHealthUi();
        }

        public void UpdateHealthUi()
        {
            GuiUnitHealth.HandleHealthChange(_currentHealth);
        }
    }
}
