using System;
using UnityEngine;

namespace Playstel
{
    public class NotifyOnDestroy : MonoBehaviour
    {
        public event Action<GameObject, NotifyOnDestroy> Destroyed;
        public GameObject instanceRef { get; set; }

        public void OnDestroy()
        {
            Destroyed?.Invoke(instanceRef, this);
        }
    }
}