using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon.StructWrapping;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Playstel
{
    [RequireComponent(typeof(NpcCharacter))]
    public class NpcCharacterCamera : MonoBehaviour
    {
        public Transform cameraPlaceholder;
        public Camera Camera;

        private Animator _cameraAnimator;
        
        private const string _dialogCameraKey = "DialogCamera";

        [Inject] private CacheUserSettings _cacheUserSettings;

        private void Awake()
        {
            _cameraAnimator = cameraPlaceholder.GetComponent<Animator>();
            UpdateRendering(Camera.gameObject);
        }

        public void ActiveCamera(bool state)
        {
            Camera.enabled = state;
        }

        public async UniTask<GameObject> CreateCamera()
        {
            var _cameraObject = await AddressablesHandler.Get(_dialogCameraKey, cameraPlaceholder);
            SetCameraTransform(_cameraObject.transform);
            UpdateRendering(_cameraObject);
            return _cameraObject;
        }
        
        public void UpdateRendering(GameObject cameraObject)
        {
            var cameraData = cameraObject.GetComponent<UniversalAdditionalCameraData>();
            cameraData.renderShadows = _cacheUserSettings.Shadows;
            cameraData.renderPostProcessing = _cacheUserSettings.PostProcessing;
        }

        private void SetCameraTransform(Transform _camera)
        {
            _camera.localRotation = new Quaternion(0, 0, 0, 1);
            _camera.localPosition = new Vector3(0, 0, 0);
        }

        private string phraseKey = "Phrase";
        public void PhraseCameraPose(bool state)
        {
            _cameraAnimator.SetBool(phraseKey, state);
        }
    }
}
