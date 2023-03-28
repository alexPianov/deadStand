using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiNavigator : MonoBehaviour
    {
        [Inject]
        private Unit _unit;
        public Slider Slider;

        private void Start()
        {
            ChangeSize(Slider);
        }

        public void ChangeSize(Slider slider)
        {
            _unit.Camera.GetCameraObserver().CameraMinimapRender
                .SetOrtographicSize(slider.value);
        }
        
        private void OnDisable()
        {
            _unit.Camera.GetCameraObserver().CameraMinimapRender.SetOrtographicSize();
        }
    }
}