using UnityEngine;
using Zenject;

namespace Playstel
{
    [RequireComponent(typeof(BoxCollider))]
    public class AreaTrigger : MonoBehaviour
    {
        [Header("Setup")]
        public UnitAreaBehaviour.Area AreaOnEnter = UnitAreaBehaviour.Area.Safe;
        public UnitAreaBehaviour.Area AreaOnExit = UnitAreaBehaviour.Area.Unsafe;
        public AreaName areaName;
        
        [Header("Owner")]
        public UnitFraction.Fraction AreaFraction = UnitFraction.Fraction.Null;
        public bool kickDifferentFraction;
        
        public enum AreaName
        {
            Church, Trade, Skull, Police, Lucky, Toxic
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Unit unit))
            {
                if (!unit.AreaBehaviour) return;
                if (!unit.photonView.IsMine) return;
                if (unit.Death.IsDead()) return;

                if (kickDifferentFraction && unit.Fraction.currentFraction != AreaFraction)
                {
                    unit.AreaBehaviour.SetAreaBehaviour
                        (UnitAreaBehaviour.Area.Dead, areaName.ToString());
                    
                    return;
                }

                unit.AreaBehaviour.SetAreaBehaviour(AreaOnEnter, areaName.ToString());
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Unit unit))
            {
                if (!unit.AreaBehaviour) return;
                if (!unit.photonView.IsMine) return;
                if (unit.Death.IsDead()) return;

                if (kickDifferentFraction && unit.Fraction.currentFraction != AreaFraction)
                {
                    unit.AreaBehaviour.SetAreaBehaviour(AreaOnExit, areaName.ToString(), false);
                    return;
                }
                
                unit.AreaBehaviour.SetAreaBehaviour(AreaOnExit, areaName.ToString());
            }
        }
    }
}