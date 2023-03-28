using System;
using Cysharp.Threading.Tasks;
using Lean.Gui;
using Playstel.UI;
using UnityEngine;

namespace Playstel
{
    public class UnitGrenadeThrow : MonoBehaviour
    {
        [Header("Refs")]
        private LeanJoystick _joystickAim;
        private GameObject _throwAimTarget;
        private LineRenderer _lineRenderer;
        private Unit _unit;

        [HideInInspector] 
        public Transform _transform, _throwAimTargetTransform;

        public bool GrenadeIsThrowed;

        private void Awake()
        {
            _transform = transform;
            _unit = GetComponent<Unit>();
        }

        public void SetAimJoystick(LeanJoystick leanJoystick)
        {
            _joystickAim = leanJoystick;
        }

        public void SetAim(GuiAimThrow aimThrow)
        {
            _throwAimTarget = aimThrow.throwAimTarget;
            _lineRenderer = aimThrow.lineRenderer;
            _lineRenderer.positionCount = _trajectorySteps + 2;
            _lineRenderer.enabled = false;

            _throwAimTargetTransform = aimThrow.throwAimTarget.transform;
            
            _throwAimTarget.SetActive(false);
        }
        
        public void CheckForThrowingMode(bool aiming)
        {
            if (!_unit.HandleItems.currentItemController) return;
            
            if (!_unit.HandleItems.currentItemController.itemGrenade)
            {
                ThrowingLine(false);
                GrenadeIsThrowed = false; 
                return;
            }

            if (aiming)
            {
                DrawTrajectory();
            }
            else
            {
                GrenadeIsThrowed = false;
            }

            ThrowingLine(aiming && !GrenadeIsThrowed);
        }

        [Header("Trajectory")]
        public AnimationCurve curve;
        private const int _trajectorySteps = 20;
        private Vector3 _itemPos;
        private Vector3 _aimPos;
        private Vector3 _step;
        private void DrawTrajectory()
        {
            _itemPos = _unit.HandleItems.currentItem.instance.transform.position;
            _aimPos = _unit.Aim.GetAimTarget().position;
            _step = (_aimPos - _itemPos) / _trajectorySteps;

            _lineRenderer.SetPosition(0, _itemPos);

            SetCurve(_itemPos, _step);

            _lineRenderer.SetPosition(_trajectorySteps + 1, _aimPos);

            _throwAimTargetTransform.position = _aimPos;
        }

        private void SetCurve(Vector3 itemPos, Vector3 step)
        {
            for (int i = 0; i < _trajectorySteps; i++)
            {
                Vector3 stepPos = itemPos + (step * i);
                stepPos.y += curve.Evaluate(i / (float) _trajectorySteps);
                _lineRenderer.SetPosition(i + 1, stepPos);
            }
        }

        public void ThrowingLine(bool state)
        {
            _lineRenderer.enabled = state;
            _throwAimTarget.SetActive(state);
        }

        public void AimJoystic(bool state)
        {
            _joystickAim.interactable = state;
        }

        public void AimJoysticVisual(bool state)
        {
            _joystickAim.gameObject.SetActive(state);
        }

        [HideInInspector]
        public bool throwCamSize;
        private const float _throwCamTimer = 2f;
        public async void ThrowCamDelay()
        {
            throwCamSize = true;
            await ThrowPending(_throwCamTimer);
            throwCamSize = false;
        }

        private const float _throwMoveTimer = 0.5f;
        public async void MoveThrowPending()
        {
            MoveEnable(false);
            await ThrowPending(_throwMoveTimer);
            MoveEnable(true);
        }

        private async UniTask ThrowPending(float time)
        {
            await UniTask.Delay(Mathf.RoundToInt(time * 1000));
        }

        public void MoveEnable(bool state)
        {
            AimJoystic(state);
            _unit.Move.CanMove(state);
        }

        private const float _throwCoefficient = 45f;
        
        public float GetThrowPower()
        {
            var distance = Vector3.Distance(_throwAimTargetTransform.position, _transform.position) * 0.1f;
            return distance * _throwCoefficient;
        }
    }
}
