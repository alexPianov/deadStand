using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class GuiButtonInteraction : UiTransparency
    {
        [Header("Refs")]
        public Canvas canvas;
        public Button button;
        public TextMeshProUGUI buttonText;

        [Header("Transform")]
        [HideInInspector] public Transform _canvasTransform;
        [HideInInspector] public Transform _targetTransform;
        [HideInInspector] public Transform _camTransform;

        public string _startText;
        
        [Inject]
        private void Construct(CameraObserver cameraObserver)
        {
            _targetTransform = transform;
            _canvasTransform = canvas.transform;
            _camTransform = cameraObserver.MainCamera.transform;
            canvas.worldCamera = cameraObserver.MainCamera;
            
            _startText = buttonText.text; 

            ActiveButton(false);
        }
        
        public void ActiveButton(bool state)
        { 
            Transparency(!state);
            
            button.interactable = state;

            if (state)
            {
                buttonText.text = null;
                buttonText.text = _startText;
            }
        }
        
        private void LateUpdate()
        {
            if (canvas.enabled && _canvasTransform)
            {
                _canvasTransform.LookAt(_targetTransform.position + _camTransform.forward);
            }
        }
    }
}
