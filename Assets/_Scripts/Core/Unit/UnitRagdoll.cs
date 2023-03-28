using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Playstel
{
    public class UnitRagdoll : MonoBehaviourPun
    {
        [Header("Refs")]
        public GameObject root;

        [Header("Bones")]
        List<Collider> colliders = new List<Collider>();
        List<Rigidbody> rigidbodys = new List<Rigidbody>();
        List<CharacterJoint> joints = new List<CharacterJoint>();

        CapsuleCollider capsuleCollider;
        Animator animator;
        NavMeshAgent agent;
        Rigidbody mainRigidbody;

        public void Awake()
        {
            SetComponents();
            SetBones();
            RagdollMode(false);
        }

        private void SetComponents()
        {
            capsuleCollider = GetComponent<CapsuleCollider>();
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            mainRigidbody = GetComponent<Rigidbody>();
        }    

        private void SetBones()
        {
            colliders = DataHandler.GetChildrens<Collider>(root);
            rigidbodys = DataHandler.GetChildrens<Rigidbody>(root);
            joints = DataHandler.GetChildrens<CharacterJoint>(root);
        }

        public void RagdollMode(bool state)
        {
            if (!state) ReturnBoneAlignment();
            
            RootRigidbodysKinematic(!state);
            //MainRigidbodyKinematic(!state);
            MainColliderIsTrigger(state);
            
            EnableNavMeshAgent(!state);
            EnableAnimator(!state);
            EnableBoneColliders(state);
        }

        public void MainColliderIsTrigger(bool state)
        {
            capsuleCollider.isTrigger = state;
        }

        public void ReturnBoneAlignment()
        {
            for (int i = 0; i < joints.Count; i++)
            {
                joints[i].enableProjection = true;
            }
        }

        public void EnableBoneColliders(bool state)
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                colliders[i].enabled = state;
            }
        }

        public void EnableUnitCollider(bool state)
        {
            capsuleCollider.enabled = state;
        }

        private void RootRigidbodysKinematic(bool state)
        {
            for (int i = 0; i < rigidbodys.Count; i++)
            {
                rigidbodys[i].isKinematic = state;
            }
        }

        private void MainRigidbodyKinematic(bool state)
        {
            if (mainRigidbody)
            {
                mainRigidbody.useGravity = !state;
                mainRigidbody.isKinematic = state;
            }
        }

        public void EnableNavMeshAgent(bool state)
        {
            return;
            if(agent) agent.enabled = state;
        }

        public void EnableAnimator(bool state)
        {
            if(animator) animator.enabled = state;
        }

        const int ragdollForceFactor = 900;
        public void AddForce(Vector3 hitPos)
        {
            Vector3 ragdollDirection = transform.position - hitPos;

            ragdollDirection = ragdollDirection.normalized;

            Vector3 force = ragdollDirection * (ragdollForceFactor * Random.Range(0.8f, 1.2f)) * 0.1f;

            Vector3 pos = hitPos + new Vector3(0, 1f, 0);

            rigidbodys[0].AddForceAtPosition(force, pos, ForceMode.Impulse);
        }
    }
}
