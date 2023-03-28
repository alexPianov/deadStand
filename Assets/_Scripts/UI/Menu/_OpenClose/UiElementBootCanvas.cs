using EventBusSystem;
using UnityEngine;

namespace Playstel
{
    public class UiElementBootCanvas : MonoBehaviour, IBootCanvasHandler
    {
        public UiTransparency UiTransparency;
        
        private void OnEnable()
        {
            EventBus.Subscribe(this);
        }
        
        private void OnDisable()
        {
            EventBus.Unsubscribe(this);
        }
        
        public void HandleCanvasTransparency(bool canvasTransparency)
        {
            UiTransparency.Transparency(canvasTransparency);
        }
    }
}