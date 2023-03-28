using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Playstel
{
    public class RoomTime : MonoBehaviour
    {
        public int roundMinutes { get; private set; }
        public float roundSeconds { get; private set; }

        public UnityEvent RoundEnd = new();
        public UnityEvent NewMinute = new();
        
        private ObscuredBool _isFinished;
        
        private ObscuredInt _maxRoundMinutes = 5;
        private ObscuredInt _maxRoundSeconds = 60;

        [Inject] private LocationInstaller _locationInstaller;
        
        private void Awake()
        {
            _locationInstaller.BindFromInstance(this);
            
            roundMinutes = _maxRoundMinutes;
            roundSeconds = _maxRoundSeconds;
        }

        private void Update()
        {
            if(_isFinished) return;
            
            roundSeconds -= Time.deltaTime;

            if (roundSeconds <= 0)
            {
                roundMinutes--;
                
                if (roundMinutes <= 0)
                {
                    RoundEnd.Invoke();
                    _isFinished = true;
                    return;
                }
                
                roundSeconds = 60;
                NewMinute.Invoke();
            }
        }
    }
}