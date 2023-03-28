using CodeStage.AntiCheat.ObscuredTypes;
using Lean.Gui;
using UniRx;
using UnityEngine;

namespace Playstel
{
    public class UnitJoystick : MonoBehaviour
    {
        private LeanJoystick _joystickAim;
        private LeanJoystick _joystickRun;

        private Unit _unit;
        private CameraObserver _cameraObserver;
        
        private ObscuredFloat _aimingRange;
        private ObscuredFloat _aimingRangeMinimum = 1f;

        private Transform _transform, _joystickTarget, 
            _joystickLookRot, _cameraTransform;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            
            _transform = transform;
            
            _joystickLookRot = CreateJoysticComponent("JoystickLookRot", transform);
        }

        private Transform CreateJoysticComponent(string componentName, Transform parent = null)
        {
            var component = new GameObject();
            component.transform.parent = parent;
            component.transform.position = transform.position;
            component.name = componentName;
            return component.transform;
        }

        public void SetJoysticTarget(Transform aimTransform)
        {
            _joystickTarget = CreateJoysticComponent("JoystickTarget", aimTransform);
        }
        
        public void SetCamera(CameraObserver cameraObserver)
        {
            _cameraObserver = cameraObserver;
            _cameraTransform = cameraObserver.MainCamera.transform;
        }

        public void SetAimJoystick(LeanJoystick joystickAim)
        {
            _joystickAim = joystickAim;
        }
        
        public void SetRunJoystick(LeanJoystick joystickRun)
        {
            _joystickRun = joystickRun;
        }

        
        #region Aim Joystick

        private Vector3 _aimLookVector = new();
        private ObscuredFloat _aimRotSpeed = 12f;
        public virtual void JoystickAim()
        {
            if(!_joystickAim) return;

            AimingViewRange();
            
            if (!_unit.Move.CanMove()) return;
            
            UpdateJoysticTransform();

            _joystickLookRot.LookAt(_joystickTarget.position);

            if (_joystickAim.isOn) _unit.Aim.AimTarget(_joystickTarget); 

            UpdateUnitRotation(_joystickLookRot.rotation, _aimRotSpeed);
        }
        
        private void UpdateUnitRotation(Quaternion rotationTarget, float rotationSpeed)
        {
            _transform.rotation = Quaternion.Lerp
                (transform.rotation, rotationTarget, 
                    Time.deltaTime * rotationSpeed);

            _transform.localEulerAngles = new Vector3(0, _transform.localEulerAngles.y, 0);
        }

        private Vector3 GetLookVector(LeanJoystick joystick)
        {
            return new Vector3(joystick.ScaledValue.x, 0, joystick.ScaledValue.y);
        }

        private Quaternion GetCameraAngle()
        {
            if (!_cameraTransform) return default;
            
            return Quaternion.Euler(0, _cameraTransform.eulerAngles.y, 0);
        }

        private const int _targetCoeff = 5;
        private const float _forwardCoeff = 0.4f;
        private void UpdateJoysticTransform()
        {
            _aimLookVector = GetCameraAngle() * GetLookVector(_joystickAim) * _aimingRange;

            _joystickTarget.position = _transform.position + _aimLookVector * _targetCoeff;

            _joystickTarget.rotation = _transform.rotation;

            CheckMinJoysticValue(_joystickAim.ScaledValue);
        }

        private const float _minJoystickValue = 0.1f;
        private void CheckMinJoysticValue(Vector2 joysticValue)
        {
            if (Mathf.Abs(joysticValue.x) <= _minJoystickValue &&
                Mathf.Abs(joysticValue.y) <= _minJoystickValue)
            {
                _joystickTarget.localPosition += _joystickTarget.forward * _forwardCoeff;
            }
        }

        private ObscuredFloat _aimingRangeCoefficient;
        public void AimingViewRange()
        {
            if(!_cameraObserver) return;
            
            _noAimingRange = false;

            _aimingRangeCoefficient = DefineViewRange() * 0.01f;
            _aimingRange = _aimingRangeMinimum + _aimingRangeCoefficient;

            _cameraObserver.NewSize(_aimingRangeCoefficient, _unit.Aim.GetAimingDistance());
        }

        private float DefineViewRange()
        {
            return _unit.HandleItems.currentItemStat.viewRange;
        }

        #endregion

        #region Run Joystick

        private Vector3 _runLookVector = new();
        private Quaternion _lookRotationTargetNoAim = new();
        private const float _runRotationSpeed = 100f;
        private const float runJoysticMagnitudeMin = 0.25f;
        public virtual void JoystickRun()
        {
            if(!_joystickRun) return;
            
            NoAimingViewRange();
            
            if (!_unit.Move.CanMove()) return;

            _runLookVector = GetLookVector(_joystickRun);

            if (_runLookVector.magnitude < runJoysticMagnitudeMin) return;

            _runLookVector = GetCameraAngle() * _runLookVector;

            _lookRotationTargetNoAim = Quaternion.LookRotation(_runLookVector);
            
            UpdateUnitRotation(_lookRotationTargetNoAim, _runRotationSpeed);
        }

        private bool _noAimingRange;
        private void NoAimingViewRange()
        {
            if(!_cameraObserver) return;
            
            _cameraObserver.StartSize();

            if (_noAimingRange) return;

            if (_unit.GrenadeThrow.throwCamSize) return;

            _noAimingRange = true;
            
            _aimingRange = _aimingRangeMinimum;
        }

        #endregion
    }
}
