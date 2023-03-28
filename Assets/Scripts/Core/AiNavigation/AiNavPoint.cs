using System;
using Playstel;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.AiNavigation
{
    public class AiNavPoint : MonoBehaviour
    {
        [Header("Orientation")] 
        public UnitAiTargetGlobal.AiOrientation Orientation;
        
        [Header("Collider")] 
        public SphereCollider SphereCollider;

        private void Start()
        {
            SphereCollider.isTrigger = true;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<UnitAiTargetGlobal>())
            {
                other.GetComponent<UnitAiTargetGlobal>().SetNavPoint(null);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            
            Gizmos.DrawSphere(transform.position, 1);
        }

        public Vector3 GetRandomPoint()
        {
            return transform.position + new Vector3(SpreadX(), 0, SpreadZ());
        }

        private float SpreadX()
        {
            return Random.insideUnitSphere.x * SphereCollider.radius;
        }

        private float SpreadZ()
        {
            return Random.insideUnitSphere.z * SphereCollider.radius;
        }

        public float GetDistance(Vector3 unitPosition)
        {
            return Vector3.Distance(transform.position, unitPosition);
        }
    }
}