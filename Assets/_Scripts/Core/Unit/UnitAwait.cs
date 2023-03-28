using System;
using EventBusSystem;
using Photon.Pun;
using UnityEngine;

namespace Playstel
{
    public class UnitAwait : MonoBehaviourPun
    {
        private Unit _unit;

        private void Start()
        {
            _unit = GetComponent<Unit>();
        }

        public void SetMaxAwaitTime(float value)
        {
            if(!photonView.IsMine || _unit.IsNPC) return;

            EventBus.RaiseEvent<IWaitHandler>
                (h => h.HandleMaxWaitTime(value));
        }

        public void Await(bool state, bool withoutPauses = false)
        {
            waitingTime = 0;

            if (!withoutPauses)
            {
                awaiting = state;
            }

            if(!state)
            {
                SetWaitingTime(0);
            }
        }

        public bool awaiting;
        
        public bool IsAwaiting()
        {
            return awaiting;
        }

        float waitingTime;
        private void Update()
        {
            if (awaiting)
            {
                waitingTime += Time.deltaTime;
                SetWaitingTime(waitingTime);
            }
        }

        private void SetWaitingTime(float value)
        {
            if(!photonView.IsMine || _unit.IsNPC) return;

            EventBus.RaiseEvent<IWaitHandler>
                (h => h.HandleWaitTime(value));
        }
    }
}
