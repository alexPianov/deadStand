using CodeStage.AntiCheat.ObscuredTypes;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Playstel
{
    public class UnitAim : MonoBehaviourPun, IPunObservable
    {
        public bool aiming;
        public bool attack;
        
        [Header("Refs")]
        public Rig aimingRig;
        public Transform aimTarget;

        protected Unit _unit;
        private Animator _animator;

        private Transform _transform;
        private ObscuredFloat _aimHeight = 1.25f;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _unit = GetComponent<Unit>();
            
            _transform = transform;
            _aimHeightPos = new Vector3(0, _aimHeight, 0);
        }
        
        public void Aiming(bool state)
        {
            aiming = state;
            
            if (!aiming) Attack(false);
        }

        public void Attack(bool state)
        {
            attack = state;
        }

        protected virtual void Update()
        {
            if (_unit.IsNPC) return; 
            
            UpdateAnimatorAim();
            AimingRigWeight(aiming);
            
            if (!photonView.IsMine) return;

            //_unit.GrenadeThrow.CheckForThrowingMode(aiming);
            
            LaserSightMode(aiming);

            if (aiming)
            {
                _unit.Joystick.JoystickAim();
            }
            else
            {
                _unit.Joystick.JoystickRun();
            }
        }

        #region Laser Sight

        public void LaserSightMode(bool state)
        {
            if (!_unit.HandleItems.currentItemController) return;
            if (!_unit.HandleItems.currentItemController.itemFirearm) return;
            if (!_unit.HandleItems.currentItemController.itemFirearm.LaserSight) return;

            var laserSight = _unit.HandleItems.currentItemController.itemFirearm.LaserSight;
            
            if (_unit.Await.IsAwaiting())
            {
                laserSight.ActiveLaserSight(false);
            }
            else
            {
                laserSight.ActiveLaserSight(state);
            }
        }

        #endregion

        #region Aim Target

        public Transform GetAimTarget()
        {
            return aimTarget;
        }
        
        public float GetAimingDistance(Vector3 targetPosition = default)
        {
            if (targetPosition == default) targetPosition = GetAimTarget().position;
            return Vector3.Distance(targetPosition, _transform.position) * 0.1f;
        }
        
        private Vector3 _targetCache = new ();
        private Vector3 _aimCache = new();
        private Vector3 _currentVelocity;
        private Vector3 _aimHeightPos;
        private ObscuredFloat _aimSmoothTime = 0.1f;
        private ObscuredFloat _aimMaxSpeed = 50;
        public void AimTarget(Transform target)
        {
            _targetCache = target.position + _aimHeightPos;

            _aimCache = aimTarget.position;

            aimTarget.position = Vector3.SmoothDamp(_aimCache, _targetCache,
                ref _currentVelocity, _aimSmoothTime, _aimMaxSpeed);

            aimTarget.LookAt(_transform.position);
        }

        #endregion

        #region Aim Mode

        private ObscuredFloat _aimAnimSpeed = 3f;
        private float _weight;
        public void AimingRigWeight(bool state)
        {
            if (state) _weight += Time.deltaTime * _aimAnimSpeed;
            if (!state) _weight -= Time.deltaTime * _aimAnimSpeed;

            if (_weight > 1) _weight = 1;
            if (_weight < 0) _weight = 0; 
            
            aimingRig.weight = _weight;
        }

        private const string aimingAnimatorKey = "Aiming";
        public void UpdateAnimatorAim()
        {
            _animator.SetBool(aimingAnimatorKey, aiming);
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (!photonView) return;

            if (stream.IsWriting)
            {
                stream.SendNext(aiming);
            }
            else
            {
                aiming = (bool) stream.ReceiveNext();
            }
        }

        #endregion
    }
}
