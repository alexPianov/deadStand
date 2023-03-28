using CodeStage.AntiCheat.ObscuredTypes;
using Lean.Gui;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

namespace Playstel
{
    public class UnitMove : MonoBehaviourPun, IPunObservable
    {
        private bool canMove;
        public bool isRunning;
        
        [Header("Animator Values")]
        public float animatorSpeed;
        public float turnAmount;
        public float forwardAmount;

        [Header("Control Stats")]
        public ObscuredFloat _middleRunSpeed;
        public ObscuredFloat _sprintSpeed;
        
        private Animator _animator;
        private Unit _unit;

        private LeanJoystick _joystickRun;
        
        private Transform _transform, _cameraTransform;
        private NavMeshAgent _navMeshAgent;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _unit = GetComponent<Unit>();
            _transform = transform;
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public void SetMoveValues(float RunSpeed, float SprintSpeed)
        {
            _middleRunSpeed = RunSpeed;
            _sprintSpeed = SprintSpeed;
        }

        public void SetCameraTransform(Transform cameraTransform)
        {
            _cameraTransform = cameraTransform;
        }

        public void SetRunJoystick(LeanJoystick runJoystick)
        {
            _joystickRun = runJoystick;
        }

        public void CanMove(bool state)
        {
            canMove = state;
        }

        public bool CanMove()
        {
            return canMove;
        }

        private void FixedUpdate()
        {
            UpdateAnimatorMove();

            if (!photonView.IsMine) return;
            if (!_joystickRun) return;
            if (!_cameraTransform) return;

            Move();
        }

        private void ResetMoveValues()
        {
            animatorSpeed = 0; forwardAmount = 0; turnAmount = 0;
        }

        [SerializeField] public float _runMagnitude;
        [SerializeField] private Vector3 _moveVector;
        [SerializeField] private bool manualRunMagnitude;
        public virtual void Move()
        {
            _runMagnitude = GetRunMagnitude(_joystickRun);
            animatorSpeed = GetAnimatorSpeed(_runMagnitude);

            if (canMove)
            {
                _moveVector = GetCameraAngles() * GetMoveVector(_joystickRun);

                turnAmount = _moveVector.x; forwardAmount = _moveVector.z;
                
                isRunning = _moveVector.magnitude > 0;

                if (isRunning)
                {
                    MoveUnit(_moveVector, _runMagnitude);
                }
            }
            else
            {
                ResetMoveValues();
            }
        }

        private const float _joystickDeadZone = 0.25f;
        private static float GetRunMagnitude(LeanJoystick joystickRun)
        {
            if (joystickRun.ScaledValue.magnitude >= _joystickDeadZone) 
                return joystickRun.ScaledValue.magnitude;
            
            return 0;
        }

        public ForceMode ForceMode = ForceMode.Acceleration;
        public Vector3 moveVectorFull;
        private const float _aimingMoveThreshold = 0.7f;
        private float runSpeed;
        private void MoveUnit(Vector3 moveVector, float runMagnitude)
        {
            if (_unit.Sprint.sprint)
            {
                runSpeed = _sprintSpeed * GetDrugBonus();
            }
            else
            {
                runSpeed = _middleRunSpeed * GetDrugBonus();
            }

            if (!_unit.Aim.aiming)
            {
                moveVector *= runSpeed;
            }

            if (_unit.Aim.aiming && runMagnitude >= _aimingMoveThreshold)
            {
                moveVector *= runSpeed / GetAimingPenalty();
            }

            if (_unit.Aim.aiming && runMagnitude < _aimingMoveThreshold)
            {
                moveVector *= 0;
                ResetMoveValues();
            }

            moveVectorFull = moveVector * Time.deltaTime * runMagnitude * GetImpactPenalty();

            _transform.Translate(moveVectorFull);
        }

        private Vector3 GetMoveVector(LeanJoystick joystickRun)
        {
            return new Vector3(joystickRun.ScaledValue.x, 0, joystickRun.ScaledValue.y).normalized;
        }

        private Quaternion GetCameraAngles()
        {
            return Quaternion.Euler(0, 0 - _transform.eulerAngles.y + _cameraTransform.eulerAngles.y, 0);
        }

        private const float _animSpeedRunCoeff = 4.1f;
        private const float _animSpeedSprintCoeff = 5.3f;
        private float GetAnimatorSpeed(float runMagnitude)
        {
            if (_unit.Sprint.sprint)
            {
                return runMagnitude * _animSpeedSprintCoeff * GetImpactPenalty();
            }
            
            return runMagnitude * _animSpeedRunCoeff * GetImpactPenalty();
        }

        private ObscuredFloat animatorSpeedAimPenalty = 1.5f;
        private float GetAimingPenalty()
        {
            return _unit.HandleItems.currentItemStat.weight * animatorSpeedAimPenalty; 
        }

        private float GetDrugBonus()
        {
            return _unit.Drugs.GetSpeedBonus();
        }

        private float GetImpactPenalty()
        {
            return _unit.Impact.GetSpeedPenalty();
        }

        private const float _sprintDampValue = 0.1f;
        private const float _aimingDampValue = 0.1f;
        private const string speedAnimatorKey = "Speed";
        private const string forwardAnimatorKey = "X";
        private const string turnAnimatorKey = "Y";
        private void UpdateAnimatorMove()
        {
            _animator.SetFloat(speedAnimatorKey, animatorSpeed, _sprintDampValue, Time.deltaTime);
            _animator.SetFloat(forwardAnimatorKey, forwardAmount, _aimingDampValue, Time.deltaTime);
            _animator.SetFloat(turnAnimatorKey, turnAmount, _aimingDampValue, Time.deltaTime);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (!photonView) return;

            if(_unit.Boot && !_unit.Boot.IsFinish) return;

            if (stream.IsWriting)
            {
                stream.SendNext(animatorSpeed);
                stream.SendNext(forwardAmount);
                stream.SendNext(turnAmount);
            }
            else
            {
                animatorSpeed = (float)stream.ReceiveNext();
                forwardAmount = (float)stream.ReceiveNext();
                turnAmount = (float)stream.ReceiveNext();
            }
        }
    }
}
