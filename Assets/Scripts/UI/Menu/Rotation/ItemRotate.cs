using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Playstel
{
    public class ItemRotate : MonoBehaviour
    {
        [Header("Slider")]
        public Slider rotationSlider;
        
        [Header("Mode")]
        public bool autoRotation;
        public float autoRotationSpeed = 50;
        private const float startAngle = 175;

        [Header("Object")]
        public GameObject RotationObject;

        [Header("Setup")]
        private const float _minSliderRotation = 50;
        private const float _maxSliderRotation = 320;
        private const int smoothTime = 60;
        private float viewRotationTime;
        public bool viewRotation;
        public Vector3 rotAngle;
        private Vector3 v = new (0,175,0);
        private UnitRenderer _unitRenderer;

        public void Awake()
        {
            UpdateRotationSlider();
        }

        public void UpdateRotationSlider()
        {
            if (!rotationSlider) return;

            rotationSlider.maxValue = _maxSliderRotation;
            rotationSlider.minValue = _minSliderRotation;
            rotationSlider.value = startAngle;
        }
        
        public void ActiveViewRotation(bool state)
        {
            viewRotation = state;
        }

        public async UniTask SetObject(GameObject rotationObject, int delay = 100)
        {
            if(rotationObject == null) return;
            
            RotationObject = rotationObject;
            
            SetObjectRenderer(rotationObject);
            ActiveViewRotation(false);
            viewRotationTime = 1f;

            v = new Vector3(0, startAngle, 0);
            var objectRotation = Quaternion.Euler(v);
            
            await UniTask.Delay(delay);
            
            if(RotationObject) RotationObject.transform.rotation = objectRotation;

            rotAngle = v;
        }

        private void SetObjectRenderer(GameObject unit)
        {
            if(unit) return;
            _unitRenderer = unit.GetComponent<UnitRenderer>();
        }

        public void Rotate(Slider slider)
        {
            if (!RotationObject) return;

            if (_unitRenderer && !_unitRenderer.renderIsActive) return;

            rotAngle = new Vector3(0, slider.value, 0);
            viewRotationTime = 0;

            ActiveViewRotation(true);
        }

        public void Update()
        {
            if(!RotationObject) return;
            
            if (viewRotation)
            {
                ViewRotation();
                return;
            }
            
            if (autoRotation)
            {
                AutoRotation();
            }
        }
        
        private void ViewRotation()
        {
            iTween.RotateUpdate(RotationObject, rotAngle, smoothTime * Time.deltaTime);

            viewRotationTime += Time.deltaTime;

            if (viewRotationTime > 0.2f)
            {
                v = rotAngle;
                viewRotation = false;
            }
        }

        private void AutoRotation()
        {
            v.x = 0;
            v.y -= 1 * Time.deltaTime * autoRotationSpeed;
            v.z = 0;

            iTween.RotateUpdate(RotationObject, v, smoothTime * Time.deltaTime);
        }

    }
}
