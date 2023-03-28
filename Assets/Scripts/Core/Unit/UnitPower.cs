using System;
using UnityEngine;

namespace Playstel
{
    public class UnitPower : MonoBehaviour
    {
        private Unit _unit;
        
        [SerializeField]
        private Unit oppositeUnit;

        private void Start()
        {
            _unit = GetComponent<Unit>();
        }

        public int GetPower()
        {
            var health = _unit.Health._currentHealth;
            var lives = _unit.Lives._currentLives;
            return lives * health;
        }

        public void SetOppositeUnit(Unit unit)
        {
            oppositeUnit = unit;
        }

        public Unit GetOppositeUnit()
        {
            return oppositeUnit;
        }
    }
}