using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EventBusSystem;
using TMPro;
using UnityEngine;

namespace Playstel
{
    public class GuiAnnounce : MonoBehaviour, IAnnounceHandler
    {
        public TextMeshProUGUI announceText;
        
        private void OnEnable()
        {
            EventBus.Subscribe(this); 
        }
        
        private void OnDisable()
        {
            EventBus.Unsubscribe(this);
        }

        private Queue<string> announceQueue = new ();

        public void HandleValue(string announce, bool skipQueue)
        {
            if (skipQueue)
            {
                ShowAnnounce(announce);
                return;
            }
            
            announceQueue.Enqueue(announce);
        }

        private void Update()
        {
            if(announceQueue.Count == 0) return;
            
            if(announceProcess) return;
            
            ShowAnnounce(announceQueue.Dequeue());
        }

        private bool announceProcess;
        private async void ShowAnnounce(string text)
        {
            announceText.CrossFadeAlpha(1, 0, false);
            announceProcess = true;
            
            announceText.text = text;
            
            await UniTask.Delay(1500);

            announceText.CrossFadeAlpha(0, 1, false); 
            announceProcess = false;
        }
    }
}