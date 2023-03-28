using System;
using Lean.Gui;
using PlayFab;
using TMPro;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class HandlerPulse : MonoBehaviour
    {
        public LeanPulse leanPulse;
        public TextMeshProUGUI pulseMessage;

        private LocalizationAgent _localizationAgent;

        public void Awake()
        {
            _localizationAgent = pulseMessage.GetComponent<LocalizationAgent>();
        }

        public void OpenTextNote(string text)
        {
            Debug.Log("Pulse Text: " + text);
            
            SetPulse();
            CheckLocalization(text);
        }

        private void SetPulse()
        {
            leanPulse.RemainingPulses = 1;
            leanPulse.RemainingTime = 1.2f;
            Debug.Log("SetPulse: " + leanPulse.RemainingPulses);
            leanPulse.Pulse();
        }
        
        private void CheckLocalization(string text)
        {
            if (_localizationAgent)
            {
                _localizationAgent.SetLocalizeFlag(text);
            }
            else
            {
                pulseMessage.text = text;
            }
        }
    }
}
