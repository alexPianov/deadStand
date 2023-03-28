using EventBusSystem;
using UnityEngine;

namespace Playstel
{
    public class CameraMinimapGizmosRotate : MonoBehaviour, IGizmosRotateHandler
    {
        public RectTransform gizmosTransform;
        
        private void OnEnable()
        {
            EventBus.Subscribe(this);
        }
        
        private void OnDisable()
        {
            EventBus.Unsubscribe(this);
        }

        public void HandleCameraRotationChange(float value)
        {
            gizmosTransform.rotation = Quaternion.Euler(90, value, 180);
        }
    }
}