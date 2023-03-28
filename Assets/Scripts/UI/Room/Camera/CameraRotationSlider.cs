using EventBusSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class CameraRotationSlider : MonoBehaviour
    {
        [Inject] private Unit _unit;

        private const int gizmosRotateAngle = 180;
        public void EditSlider(Slider slider)
        {
            _unit.Camera.GetCameraObserver().EditCameraAngleY(slider.value * 10);

            EventBus.RaiseEvent<IGizmosRotateHandler>
                (h => h.HandleCameraRotationChange
                    (slider.value * 10 + gizmosRotateAngle));
        }
    }
}