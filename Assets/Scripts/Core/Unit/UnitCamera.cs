using System;
using Cysharp.Threading.Tasks;
using EventBusSystem;
using Photon.Pun;
using Playstel.UI;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UnitCamera : MonoBehaviourPun
    {
        public GuiUnitInfoCanvas UnitInfoCanvas;
        private Camera _mainCamera;
        private CameraObserver _cameraObserver;
        private Transform _mainCameraTransform;
        
        private const bool orthographic = false;
        private const int fieldOfView = 25;

        [Inject]
        private void SetCameraObserver(CameraObserver cameraObserver)
        {
            if (photonView.IsMine)
            {
                SetupCamera(cameraObserver);
                SetCameraToComponents();
                UpdateCameraSettings();
                UpdateMinimapRenderTexture();
            }

            BindCameraToUnit();
            SetupUnitInfoUi(cameraObserver);
        }
        
        private void BindCameraToUnit()
        {
            GetComponent<Unit>().Camera = this;
        }

        public CameraObserver GetCameraObserver()
        {
            return _cameraObserver;
        }
        
        private void SetupUnitInfoUi(CameraObserver cameraObserver)
        {
            UnitInfoCanvas = GetComponentInChildren<GuiUnitInfoCanvas>();
            UnitInfoCanvas.SetTargetToFollow(transform, cameraObserver.MainCamera);
        }

        private void SetupCamera(CameraObserver cameraObserver)
        {
            _cameraObserver = cameraObserver;
            _mainCamera = cameraObserver.MainCamera;
            
            _cameraObserver.Follow(transform);
            _cameraObserver.ActiveCamera(true);
            
            _mainCamera.orthographic = orthographic;
            _mainCamera.fieldOfView = fieldOfView;
            _mainCameraTransform = _mainCamera.transform;
        }

        public void ActiveCamera(bool state)
        {
            _mainCamera.enabled = state;
        }

        private void SetCameraToComponents()
        {
            GetComponent<UnitMove>().SetCameraTransform(_mainCameraTransform);
            GetComponent<UnitJoystick>().SetCamera(_cameraObserver);
        }

        private async UniTask UpdateMinimapRenderTexture()
        {
            await _cameraObserver.CameraMinimapRender.CreateRenderTexture();
        }

        public void UpdateCameraSettings()
        {
            _cameraObserver.UpdateRendering();
        }
    }
}
