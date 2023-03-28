using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Pathfinding;
using Pathfinding.Examples;
using Pathfinding.RVO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Playstel
{
    public class UnitAiMove : MonoBehaviourPun, IPunObservable
    {
        //[SerializeField] private GameObject newDestination;
        private float controllerUpdateTime;
        private float controllerMaxUpdateTime = 0.2f;

        private Transform _transform;
        public int viewRange = 6;
        public int attackRange = 3;
        
        public int runSpeed;
        public int sprintSpeed;
        
        public bool targetInSightRange, targetInAttackRange;
        public LayerMask whatIsGround;
        
        [Header("Refs")]
        private Animator animator;
        private NavMeshAgent agent;
        private UnitAiTargetLocal localTargetReceiver;
        private UnitAiTargetGlobal globalTargetReceiver;
        private UnitAiAim aim;
        private RVOExampleAgent rvoAgent;

        [Header("Targets")]
        public Transform localTarget;
        private Vector3 globalTarget;
        public bool globalTargetPointIsSet;
        ObscuredFloat walkPointRange = 8;
        private Unit _unit;
        private RVOController _rvoController;
        private float startRadius;
        
        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            aim = GetComponent<UnitAiAim>();
            _unit = GetComponent<Unit>();
            localTargetReceiver = GetComponent<UnitAiTargetLocal>();
            globalTargetReceiver = GetComponent<UnitAiTargetGlobal>();
            rvoAgent = GetComponent<RVOExampleAgent>();
            _rvoController = GetComponent<RVOController>();
            
            _transform = transform;
            startRadius = _rvoController.radius;
        }

        private bool _active;
        public void Active(bool state)
        {
            _active = state;

            if (!state)
            {
                _rvoController.locked = true;
                _rvoController.radius = 0;
                localTarget = null;
                rvoAgent.maxSpeed = 0;
            }
            else
            {
                _rvoController.locked = false;
                _rvoController.radius = startRadius;
                rvoAgent.maxSpeed = runSpeed;
            }
        }

        public void SetMoveValues(int run, int sprint)
        {
            runSpeed = run;
            sprintSpeed = sprint;
            if(agent) agent.speed = runSpeed;
            Active(true);
        }

        private void Update()
        {
            if(!_active) return;
            
            controllerUpdateTime += Time.deltaTime;
            
            if (controllerUpdateTime < controllerMaxUpdateTime) return;
            
            controllerUpdateTime = 0;
            
            var targetLayer = localTargetReceiver.GetTargetLayer();
            
            targetInSightRange = Physics.CheckSphere(_transform.position, viewRange, targetLayer);
            targetInAttackRange = Physics.CheckSphere(_transform.position, attackRange, targetLayer);

            if (targetInSightRange)
            {
                globalTargetReceiver.SetNavPoint(null);
                localTarget = localTargetReceiver.GetLocalTarget();
            }

            aim.aiming = targetInSightRange;
            aim.attack = targetInAttackRange;

            if (targetInAttackRange)
            {
                var firearm = _unit.HandleItems.GetFirearm();
                
                if (firearm)
                {
                    var layer = firearm.LaserSight.GetColliderLayer();

                    targetInAttackRange = layer.value == localTargetReceiver.GetTargetLayerInt();
                }
            }
            
            if (!targetInSightRange && !targetInAttackRange)
            {
                SetAnimAndSpeed(SpeedModes.GlobalTarget);
                GoToGlobalTarget();
            }
            
            if (targetInSightRange && !targetInAttackRange)
            {
                SetAnimAndSpeed(SpeedModes.LocalTarget);
                if (localTarget) Go(localTarget.position); 
            }
            
            if (targetInSightRange && targetInAttackRange)
            {
                SetAnimAndSpeed(SpeedModes.Idle);
            }
        }

        private void LateUpdate()
        {
            if (localTarget && targetInSightRange)
            {
                LootAtTarget(localTarget.position);
            }
        }

        private void GoToGlobalTarget()
        {
            if (targetInAttackRange) return;
            
            globalTarget = globalTargetReceiver.GetGlobalTargetPosition();
            
            Vector3 distanceToWalkPoint = transform.position - globalTarget;
            
            if (distanceToWalkPoint.magnitude > 2f)
            {
                Go(globalTarget);
            }
        }
        
        private void Go(Vector3 targetPosition)
        {
            if(targetPosition == Vector3.negativeInfinity) return;
            if(targetPosition == Vector3.positiveInfinity) return;
            
            rvoAgent.SetTarget(targetPosition);
        }

        private void LootAtTarget(Vector3 target)
        {
            _transform.LookAt(target);
        }
        
        enum SpeedModes
        {
            Idle, GlobalTarget, LocalTarget
        }

        string speedKey = "Speed";
        string speedX = "X";
        private void SetAnimAndSpeed(SpeedModes mode)
        {
            if(mode == SpeedModes.Idle)
            {
                if(agent) agent.speed = 0;
                rvoAgent.maxSpeed = 0;
                animator.SetFloat(speedKey, 0);
                animator.SetFloat(speedX, 0);
            }

            if (mode == SpeedModes.GlobalTarget)
            {
                if(agent) agent.speed = runSpeed;
                rvoAgent.maxSpeed = runSpeed;

                var speed = rvoAgent.desiredSpeed;
                if (speed < 0) speed = 0;
                if (speed > runSpeed) speed = runSpeed;
                
                animator.SetFloat(speedKey, speed + 1);
                animator.SetFloat(speedX, 1);
            }

            if (mode == SpeedModes.LocalTarget)
            {
                if(agent) agent.speed = runSpeed + 1;
                rvoAgent.maxSpeed = runSpeed;

                var speed = rvoAgent.desiredSpeed;
                if (speed < 0) speed = 0;
                if (speed > runSpeed) speed = runSpeed;
                
                animator.SetFloat(speedKey, speed + 1);
                animator.SetFloat(speedX, 1);
            }
        }

        private float weightCoefficient = 0.1f;
        public IEnumerator Speed(float value, float time, bool reduce)
        {
            CancelInvoke();

            if (!agent) yield break;
            
            if (reduce)
                agent.speed = runSpeed - value;
            else
                agent.speed = runSpeed + value;

            yield return new WaitForSeconds(time);

            agent.speed = runSpeed;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (!photonView) return;

            if (!agent) return;

            if (stream.IsWriting)
            {
                stream.SendNext(agent.speed);
            }
            else
            {
                agent.speed = (float)stream.ReceiveNext();
            }
        }
    }
}