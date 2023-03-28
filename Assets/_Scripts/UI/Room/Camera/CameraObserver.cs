using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;
using Random = UnityEngine.Random;

namespace Playstel
{
    public class CameraObserver : MonoBehaviourPun
    {
        [Header("Settings")]
        public float followSpeed = 10f;
        public float sizeSmoothTime = 10f;
        public float aimDistanceThreshold = 0.55f;
        private float ActualHeight = 0.0f;

        [Header("Refs")]
        public Camera MainCamera;
        public Transform CameraOffset;
        public GuiMinimapOffset MinimapCameraOffset;
        public CameraMinimapRender CameraMinimapRender;

        [Header("Setup")]
        public Transform targetToFollow;

        private const float TargetHeight = 0.0f;
        private const float targetHeightOffset = -1.0f;
        private const float HeightSpeed = 1.0f;
        private Transform _transform;
        private bool dialogMode;
        private bool heightOffset;
        
        [Inject] private CacheUserSettings _cacheUserSettings;
        
        [Inject]
        private void Construct(LocationInstaller _locationInstaller)
        {
            _locationInstaller.BindFromInstance(this);
        }
        
        public void Start()
        {
            actualShakeTimer = shakeTimer;
            minSize = defaultSize;
            MainCamera.fieldOfView = defaultSize;
            _transform = transform;
        }

        public void UpdateRendering()
        {
            var cameraData = MainCamera.GetComponent<UniversalAdditionalCameraData>();
            cameraData.renderShadows = _cacheUserSettings.Shadows;
            cameraData.renderPostProcessing = _cacheUserSettings.PostProcessing;
            SetBlood(_cacheUserSettings.Blood);
        }
        
        private const string BloodLayerName = "Blood";
        private void SetBlood(bool state)
        {
            if (state)
            {
                MainCamera.cullingMask = MainCamera.cullingMask 
                                         | (1 << LayerMask.NameToLayer(BloodLayerName));
            }
            else
            {
                MainCamera.cullingMask = MainCamera.cullingMask 
                                         & ~(1 << LayerMask.NameToLayer(BloodLayerName));
            }
        }

        public void ActiveCamera(bool state)
        {
            gameObject.SetActive(state);
        }

        public void Follow(Transform character)
        {
            targetToFollow = character;
            MinimapCameraOffset.SetTargetToFollow(character);
        }

        public void FollowKiller(int killerId)
        {
            if (PhotonView.Find(killerId).TryGetComponent(out Unit unitKiller))
            {
                Follow(unitKiller.transform);
                SetMaxZoom(true);
            }
        }

        #region Update

        private void FixedUpdate()
        {
            if(dialogMode) return;
            if (heightOffset) HeightOffset();
            CameraFollow();
            ShakePending();
            DynamicSize();
        }

        private void CameraFollow()
        {
            if(!targetToFollow) return;
            
            _transform.position = Vector3.Lerp(_transform.position, targetToFollow.position,
                followSpeed * Time.deltaTime);
        }

        private void HeightOffset()
        {
            ActualHeight = Mathf.Lerp(ActualHeight, TargetHeight + targetHeightOffset,
                Time.deltaTime * HeightSpeed);

            CameraOffset.localPosition = new Vector3(0.0f, 0.0f, ActualHeight);
        }

        float damp;
        private void DynamicSize()
        {
            if(recentSize == newSize) return;
            
            MainCamera.fieldOfView = Mathf.SmoothDamp
                (recentSize, newSize, ref damp, sizeSmoothTime * Time.deltaTime);
        }
        
        public void EditCameraAngleY(float rotationY)
        {
            CameraOffset.rotation = Quaternion.Euler(45, rotationY, 0);
        }

        #endregion

        #region Size

        [Header("Size Settings")]
        private float minSize;
        private float sizeChangeCoefficient = 4f;
        private const int defaultSize = 25;
        private const float safeAreaSize = 18f;

        [Header("Dynamic Values")]
        public float newSize;
        public float recentSize;

        [Header("Thresholds")]
        public float minWeaponRange = 0.28f;
        public float maxWeaponRange = 0.80f;

        public void NewSize(float weaponRange, float aimDistance)
        {
            recentSize = MainCamera.fieldOfView;

            if (weaponRange < minWeaponRange) return;

            if (aimDistance < aimDistanceThreshold) aimDistance = 0.01f;

            if (weaponRange > maxWeaponRange) weaponRange = maxWeaponRange;

            newSize = minSize + weaponRange * aimDistance * sizeChangeCoefficient;
        }

        public void StartSize()
        {
            recentSize = MainCamera.fieldOfView;
            newSize = minSize;
        }

        public void SetMaxZoom(bool state)
        {
            minSize = state ? safeAreaSize : defaultSize;
            newSize = minSize;
        }

        #endregion

        #region Shakes

        [Header("Movement Shake")]
        public float movShaking = 1.0f;
        private Vector3 randomShakePos = Vector3.zero;

        [Header("Shooting Shake")]
        public bool isShaking = false;
        public float shakeFactor = 3f;
        public float shakeTimer = .2f;
        public float shakeSmoothness = 5f;
        [HideInInspector] public float actualShakeTimer = 0.2f;

        [Header("Explosion Shake")]
        public bool isExpShaking = false;
        public float shakeExpFactor = 5f;
        public float shakeExpTimer = 1f;
        public float shakeExpSmoothness = 3f;
        [HideInInspector] public float actualExpShakeTimer = 1f;

        public void ShakePending()
        {
            if (isShaking && !isExpShaking)
            {
                if (actualShakeTimer >= 0.0f)
                {
                    actualShakeTimer -= Time.deltaTime;
                    Vector3 newPos = _transform.localPosition + CalculateRandomShake(shakeFactor, false);
                    _transform.localPosition = Vector3.Lerp(_transform.localPosition, newPos, shakeSmoothness * Time.deltaTime);
                }
                else
                {
                    isShaking = false;
                    actualShakeTimer = shakeTimer;
                }
            }

            else if (isExpShaking)
            {
                if (actualExpShakeTimer >= 0.0f)
                {
                    actualExpShakeTimer -= Time.deltaTime;
                    Vector3 newPos = _transform.localPosition + CalculateRandomShake(shakeExpFactor, true);
                    _transform.localPosition = Vector3.Lerp(_transform.localPosition, newPos, shakeExpSmoothness * Time.deltaTime);
                }
                else
                {
                    isExpShaking = false;
                    actualExpShakeTimer = shakeExpTimer;
                }
            }
        }

        public Vector3 CalculateRandomShake(float shakeFac, bool isExplosion)
        {
            randomShakePos = new Vector3(Random.Range(-shakeFac, shakeFac),
                Random.Range(-shakeFac, shakeFac), Random.Range(-shakeFac, shakeFac));

            if (isExplosion)
                return randomShakePos * (actualExpShakeTimer / shakeExpTimer);
            else
                return randomShakePos * (actualShakeTimer / shakeTimer);
        }

        public void Shake(float factor, float duration)
        {
            isShaking = true;
            shakeFactor = factor;
            shakeTimer = duration;
            actualShakeTimer = shakeTimer;
        }

        public void ExplosionShake(float factor, float duration)
        {
            isExpShaking = true;
            shakeExpFactor = factor;
            shakeExpTimer = duration;
            actualExpShakeTimer = shakeExpTimer;
        }

        #endregion
    }
}
