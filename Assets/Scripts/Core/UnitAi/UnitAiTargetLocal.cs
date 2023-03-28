using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Playstel
{
    public class UnitAiTargetLocal : MonoBehaviour
    {
        [Header("Setup")]
        public ObscuredBool receiveTargets = true;
        public ObscuredFloat currentUpdateTime;
        public LayerMask LayerRed;
        public LayerMask LayerBlue;
        public int viewRange = 5;
        public float updateTime = 1;

        [Header("Receiver")]
        public List<GameObject> localTargets = new List<GameObject>();
        private List<string> tags = new List<string>() { "Player" };
        
        private Transform _transform;
        private Unit _unit;

        public void Awake()
        {
            _transform = transform;
            _unit = GetComponent<Unit>();
        }

        public void ReceiveTargets(bool state)
        {
            receiveTargets = state;

            if (!state) ClearLocalTargets();
        }

        public void Update()
        {
            if (receiveTargets)
            {
                currentUpdateTime += Time.deltaTime;

                if (currentUpdateTime < updateTime) return;

                currentUpdateTime = 0;
                UpdateLocalTargets();
            }
        }

        public LayerMask GetTargetLayer()
        {
            if (_unit.Fraction.currentFraction == UnitFraction.Fraction.Blue)
            {
                return LayerRed;
            }
            
            if (_unit.Fraction.currentFraction == UnitFraction.Fraction.Red)
            {
                return LayerBlue;
            }

            return 0;
        }
        
        public int GetTargetLayerInt()
        {
            if (_unit.Fraction.currentFraction == UnitFraction.Fraction.Blue)
            {
                return 20;
            }
            
            if (_unit.Fraction.currentFraction == UnitFraction.Fraction.Red)
            {
                return 21;
            }

            return 0;
        }

        Collider[] colliders = new Collider[10];
        public void UpdateLocalTargets()
        {
            ClearLocalTargets();

            Physics.OverlapSphereNonAlloc(_transform.position, viewRange, colliders, GetTargetLayer());

            for (int i = 0; i < colliders.Length; i++)
            {
                if (!colliders[i]) continue;

                if (tags.Contains(colliders[i].tag))
                {
                    if(colliders[i].gameObject.layer == gameObject.layer) continue;
                    localTargets.Add(colliders[i].gameObject);
                }
            }
        }

        public void ClearLocalTargets()
        {
            localTargets.Clear();
        }

        public Transform GetLocalTarget()
        {
            if(localTargets.Count > 0)
            {
                return localTargets[0].transform;
            }

            return null;
        }
    }
}