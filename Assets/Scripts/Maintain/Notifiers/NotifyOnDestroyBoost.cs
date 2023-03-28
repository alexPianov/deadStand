using System;
using UnityEngine;

namespace Playstel
{
    public class NotifyOnDestroyBoost : MonoBehaviour
    {
        public event Action Destroyed;

        public void OnDestroy()
        {
            Destroyed?.Invoke();
        }
    }
}