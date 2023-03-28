using Cysharp.Threading.Tasks;
using Playstel.UI;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UnitAiCamera : MonoBehaviour
    {
        public GuiUnitInfoCanvas UnitInfoCanvas;
        private Camera _mainCamera;
        private CameraObserver _cameraObserver;
        private Transform _mainCameraTransform;

        [Inject]
        private void SetCameraObserver(CameraObserver cameraObserver)
        {
            SetupUnitInfoUi(cameraObserver);
        }

        private void SetupUnitInfoUi(CameraObserver cameraObserver)
        {
            UnitInfoCanvas = GetComponentInChildren<GuiUnitInfoCanvas>();
            UnitInfoCanvas.SetTargetToFollow(transform, cameraObserver.MainCamera);
        }
    }
}