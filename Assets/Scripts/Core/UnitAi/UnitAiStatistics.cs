using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Playstel
{
    public class UnitAiStatistics : MonoBehaviour
    {
        private ObscuredInt _frags;
        private ObscuredInt _deaths;
        private ObscuredInt _level;

        private void Start()
        {
            _level = Random.Range(2, 35);
        }

        public void AddFrag()
        {
            _frags++;
        }

        public void AddDeath()
        {
            _deaths++;
        }

        public int GetFrags()
        {
            return _frags;
        }

        public int GetDeaths()
        {
            return _deaths;
        }

        public int GetLevel()
        {
            return _level;
        }
    }
}