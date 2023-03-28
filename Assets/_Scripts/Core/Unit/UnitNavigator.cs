using Photon.Pun;
using UnityEngine;

namespace Playstel
{
    public class UnitNavigator : MonoBehaviourPun
    {
        [Header("Target")]
        public Transform navigationTarget;

        [Header("Navigation")]
        public GameObject navigationArrow;
        public TextMesh distanceToTarget;

        [Header("On Reaching")]
        public bool disableAtReach;
        public int maxApproach;

        [Header("Distance")]
        public bool showDistance;

        public void Start()
        {
            if (!PhotonNetwork.InRoom)
            {
                Destroy(navigationArrow);
                Destroy(this);
            }

            if (distanceToTarget) distanceToTarget.gameObject.SetActive(showDistance);
            Disable();
        }

        public void SetTarget(Transform target)
        {
            navigationTarget = target;
            navigationArrow.SetActive(true);
        }

        public void CleatTarget()
        {
            navigationTarget = null;
        }

        public void Disable()
        {
            if(!navigationArrow) return;
            
            if (navigationArrow.activeInHierarchy)
                navigationArrow.SetActive(false);
        }    

        private void LateUpdate()
        {
            if (!navigationTarget) { Disable(); return; }

            navigationArrow.transform.LookAt(navigationTarget.position);

            if (showDistance)
            {
                distanceToTarget.text = Mathf.RoundToInt
                    (Vector3.Distance(navigationTarget.position, transform.position)).ToString();
            }

            if (disableAtReach)
            {
                if (Vector3.Distance(navigationTarget.position, transform.position) <= maxApproach)
                {
                    navigationTarget = null;
                    Disable();
                }
            }
        }

    }
}
