using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel.UI
{
    public class GuiUnitInfoCanvas : MonoBehaviour
    {
        [Header("Refs")]
        public Canvas canvas;

        public Transform _canvasTransform;
        public Transform _cameraTransform;
        public Transform _targetToFollow;

        public void SetTargetToFollow(Transform targetToFollow, Camera levelCamera)
        {
            _targetToFollow = targetToFollow;
            _cameraTransform = levelCamera.transform;
            _canvasTransform = canvas.transform;
            canvas.worldCamera = levelCamera;
            Active(true);
        }

        public void Active(bool state)
        {
            canvas.enabled = state;
        }

        public void LateUpdate()
        {
            if (!_canvasTransform) return;
            if (!_cameraTransform) return;

            _canvasTransform.LookAt(_targetToFollow.position + _cameraTransform.forward);
        }
    }
}
