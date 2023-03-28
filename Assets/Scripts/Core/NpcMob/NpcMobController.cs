using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Playstel
{
    public class NpcMobController : MonoBehaviourPun, IPunObservable
    {
        [Header("Refs")]
        Animator animator;
        NavMeshAgent agent;

        [HideInInspector]
        public NpcMobStat mobStat;
        NpcMobBuilder mobBuilder;
        UnitHealth mobHealth;
        NpcMobReceiver receiver;
        Transform _transform;
        private UnitMove _unitMove;

        public List<Transform> destinations = new List<Transform>();

        public void Awake()
        {
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            mobStat = GetComponent<NpcMobStat>();
            mobHealth = GetComponent<UnitHealth>();
            mobBuilder = GetComponent<NpcMobBuilder>();
            receiver = GetComponent<NpcMobReceiver>();
            _unitMove = GetComponent<UnitMove>();

            _transform = transform;
        }

        public void Restart()
        {
            agent.speed = mobStat.runSpeed;
            // mobHealth.UpdateHealth(mobStat.health);
            // mobHealth.SetMaxHealth(mobStat.health);

            receiver.ClearTargets();
        }

        #region Move

        public LayerMask whatIsGround, whatIsPlayer;

        //Patroling
        public Vector3 walkPoint;
        bool walkPointSet;
        ObscuredFloat walkPointRange = 8;

        //States
        public ObscuredBool playerInSightRange, playerInAttackRange;

        float controllerUpdateTime;
        float controllerMaxUpdateTime = 0.5f;

        private void Update()
        {
            if (!_unitMove.CanMove() || !receiver) return; 

            controllerUpdateTime += Time.deltaTime;

            if (playerInAttackRange && playerInSightRange) Attack();

            if (controllerUpdateTime < controllerMaxUpdateTime) return;

            controllerUpdateTime = 0;

            playerInSightRange = Physics.CheckSphere(_transform.position, mobStat.viewRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(_transform.position, mobStat.attackRange, whatIsPlayer);

            if (!playerInSightRange && !playerInAttackRange) Patroling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            //if (playerInAttackRange && playerInSightRange) Attack();
        }

        public void LateUpdate()
        {
            if (!_unitMove.CanMove() || !receiver) return; 
            if (playerInSightRange) LootAtTarget();
        }

        private void ChasePlayer()
        {
            SetAnimAndSpeed(SpeedModes.Chase);

            var target = receiver.GetTarget();

            if (target)
            {
                agent.SetDestination(target.transform.position);
            }
        }

        private void Patroling()
        {
            SetAnimAndSpeed(SpeedModes.Patroling);

            if (!walkPointSet) SearchWalkPoint();

            if (walkPointSet)
                agent.SetDestination(walkPoint);

            Vector3 distanceToWalkPoint = transform.position - walkPoint;

            if (distanceToWalkPoint.magnitude < 1f)
                walkPointSet = false;
        }
        private void SearchWalkPoint()
        {
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
                walkPointSet = true;
        }


        enum SpeedModes
        {
            Idle, Patroling, Chase
        };

        string speedKey = "Speed";
        private void SetAnimAndSpeed(SpeedModes mode)
        {
            Debug.Log("SetAnimAndSpeed: " + mode.ToString());

            if(mode == SpeedModes.Idle)
            {
                agent.speed = 0;
                animator.SetFloat(speedKey, 0);
            }

            if (mode == SpeedModes.Patroling)
            {
                agent.speed = 1;
                animator.SetFloat(speedKey, 1);
            }

            if (mode == SpeedModes.Chase)
            {
                agent.speed = mobStat.runSpeed;
                animator.SetFloat(speedKey, 6);
            }
        }

        #endregion

        #region Attack

        private float attackTime;
        private float maxAttackTime = 0.5f;
        private void Attack()
        {
            attackTime += Time.deltaTime;

            if (attackTime > maxAttackTime)
            {
                Debug.Log("Attack");
                attackTime = 0;
                LootAtTarget();

                animator.SetTrigger("Attack");
                //animator.Play("Attack", 1);
            }
        }
        
        private void LootAtTarget()
        {
            var target = receiver.GetTarget();

            if (target)
            {
                _transform.LookAt(target.position);
            }
        }

        public void Bite()
        {
            Debug.Log("Bite");
/*
            if (!gameObject.activeInHierarchy) return;

            if (receiver.targets.Count < 1) return;

            Vector3 distance = _transform.position - receiver.targets[0].transform.position;

            if (distance.magnitude > mobStat.attackRange) return;

            if (!receiver.targets[0]) return;
            
            else SendDamage();*/
        }

        private void SendDamage()
        {
            Debug.Log("Send Damage");
/*
            var instance = receiver.targets[0].GetComponent<UnitHealth>();

            if (instance.currentHealth <= 0) { receiver.targets.Remove(receiver.targets[0]); return; }

            var listener = receiver.targets[0].GetComponent<UnitCallbackListener>();

            listener.ReceiveDamage
                (mobStat.damage, _transform.position, mobStat.impact, photonView.ViewID);

            if (listener.GetComponent<UnitHealth>().currentHealth < mobStat.damage)
            {
                receiver.targets.Remove(receiver.targets[0]);
            }

            mobAudio.PlayRandom(UnitSFX.Sounds.Atk, UnitAudio.Type.Weapon);*/
        }

        #endregion

        #region Change Speed

        [HideInInspector] public readonly float weightCoefficient = 0.1f;
        public IEnumerator Speed(float value, float time, bool reduce)
        {
            CancelInvoke();

            if (reduce)
                agent.speed = mobStat.runSpeed - value;
            else
                agent.speed = mobStat.runSpeed + value;

            yield return new WaitForSeconds(time);

            agent.speed = mobStat.runSpeed;
        }

        #endregion

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (!photonView) return;

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
