using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    public class NpcMobReceiver : MonoBehaviour
    {
        [Header("Refs")]
        NpcMobController controller;
        NpcMobStat mobStat;

        [Header("Setup")]
        public ObscuredBool receiveTargets = true;
        public ObscuredFloat currentUpdateTime;
        public LayerMask requiredLayer = 8;

        [Header("Receiver")]
        //public List<GameObject> targets = new List<GameObject>();
        List<string> targetTags = new List<string>() { "Player" };
        Transform _transform;

        public void Awake()
        {
            mobStat = GetComponent<NpcMobStat>();
            controller = GetComponent<NpcMobController>();
            _transform = transform;
        }

        public void Update()
        {
            if (receiveTargets)
            {
                if (!controller) return;

                currentUpdateTime += Time.deltaTime;

                if (currentUpdateTime < mobStat.updateTargetTime) return;

                currentUpdateTime = 0;
                UpdateTargets();
            }
        }

        Collider[] colliders = new Collider[10];
        private Transform mainTarget;
        public void UpdateTargets()
        {
            ClearTargets();

            Physics.OverlapSphereNonAlloc(_transform.position,
                mobStat.viewRange, colliders, requiredLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                if (!colliders[i]) continue;

                if (targetTags.Contains(colliders[i].tag))
                {
                    //targets.Add(colliders[i].gameObject);
                    mainTarget = colliders[i].gameObject.transform;
                }
            }
        }

        public void ClearTargets()
        {
            //targets.Clear();
            mainTarget = null;
        }

        public Transform GetTarget()
        {
            return mainTarget;
            
            /*
            if(targets.Count > 0)
            {
                return targets[0];
            }

            return null;*/
        }
    }
}
