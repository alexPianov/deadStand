using System;
using TMPro;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class GuiRoundTime : UIElementLoadByType
    {
        [SerializeField] private Timer timer;
        [Inject] private RoomTime roomTime;

        private void Start()
        {
            roomTime.RoundEnd.AddListener(FinishRound);
            roomTime.NewMinute.AddListener(UpdateTimer);
            
            UpdateTimer();
        }
        private void FinishRound()
        {
            Load();
        }

        private void UpdateTimer()
        {
            timer.minutes = roomTime.roundMinutes;
            timer.seconds = (int)roomTime.roundSeconds;
        }
    }
}