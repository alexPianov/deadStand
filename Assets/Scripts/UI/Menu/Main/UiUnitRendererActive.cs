using Cysharp.Threading.Tasks;
using EventBusSystem;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiUnitRendererActive : MonoBehaviour
    {
        [Inject]
        private Unit _unit;
        
        public async void ActiveRenderer(bool state)
        {
            await UniTask.WaitUntil(() => _unit);
            await UniTask.WaitUntil(() => _unit.Renderer);
            
            _unit.Renderer.Active(state);

            EventBus.RaiseEvent<IRotationSliderMode>(h => 
                h.HandlerActiveRotationSlider(state));
        }
    }
}