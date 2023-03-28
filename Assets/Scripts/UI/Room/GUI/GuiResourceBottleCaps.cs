using Cysharp.Threading.Tasks;
using EventBusSystem;

namespace Playstel
{
    public class GuiResourceBottleCaps : GuiResource, IBottleCapsHandler
    {
        private async void OnEnable()
        {
            EventBus.Subscribe(this);

            await UniTask.WaitUntil(() => Unit);

            Unit.Currency.UpdateUi(ItemInfo.Currency.BC);

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