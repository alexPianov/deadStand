using Cysharp.Threading.Tasks;
using EventBusSystem;
using UnityEngine;

namespace Playstel
{
    public class GuiResourceMedKits : GuiResource, IMedKitsHandler
    {
        private async void OnEnable()
        {
            EventBus.Subscribe(this);

            await UniTask.WaitUntil(() => Unit);

            Unit.Health.UpdateHealthUi();
            Unit.Lives.UpdateLivesUi();
            await UniTask.Delay(500);
            ShakeMode(true);
        }
        
        private void OnDisable()
        {
            EventBus.Unsubscribe(this);
        }
        
        public void HandleValue(int amount)
        {
            EditValue(amount);
        }
    }
}