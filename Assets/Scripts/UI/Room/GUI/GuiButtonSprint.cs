using Cysharp.Threading.Tasks;
using EventBusSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class GuiButtonSprint : MonoBehaviour, ISprintHandler
    {
        [Inject]
        private Unit _unit;

        private Slider enduranceSlider;

        private async void OnEnable()
        {
            EventBus.Subscribe(this);

            await UniTask.WaitUntil(() => _unit);

            _unit.Sprint.UpdateUi();
        }
        
        private void OnDisable()
        {
            EventBus.Unsubscribe(this);
        }

        private void Awake()
        {
            BindSlider();
            BindButtonListener();
        }

        private void BindSlider()
        {
            enduranceSlider = GetComponentInChildren<Slider>();
        }

        private void BindButtonListener()
        {
            var button = GetComponent<Lean.Gui.LeanButton>();
            button.OnDown.AddListener(SprintOn);
            button.OnUp.AddListener(SprintOff);
        }

        private void SprintOn()
        {
            _unit.Sprint.Sprint(true);
        }

        private void SprintOff()
        {
            _unit.Sprint.Sprint(false);
        }

        public void HandleSprintMaxValue(int maxValue)
        {
            enduranceSlider.maxValue = maxValue;
        }

        public void HandleSprintChange(float sprintValue)
        {
            enduranceSlider.value = sprintValue;
        }
    }
}
