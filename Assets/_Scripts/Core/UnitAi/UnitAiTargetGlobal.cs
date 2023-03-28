using System;
using Core.AiNavigation;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UnitAiTargetGlobal : MonoBehaviour
    {
        [SerializeField] private Vector3 globalTarget;
        [SerializeField] private Unit oppositeUnit;
        [SerializeField] private AiNavPoint lastNavPoint;
        
        [Inject] private RoomPlayers _roomPlayers;
        [Inject] private AiNavMap _navMap;

        public AiOrientation orientation;
        public enum AiOrientation
        {
            None, Right, Left
        }
        
        private Unit _unit;
        private void Start()
        {
            _unit = GetComponent<Unit>();
        }
        
        public Vector3 GetGlobalTargetPosition()
        {
            if (!_roomPlayers || !_unit.Fraction)
            {
                Debug.Log("Room players of fraction is null");
                return new Vector3();
            }

            if (lastNavPoint == null)
            {
                if (!oppositeUnit)
                {
                    oppositeUnit = _roomPlayers
                        .GetOppositeUnit(_unit.Fraction.currentFraction, _unit);
                    
                    if (oppositeUnit) globalTarget = oppositeUnit.transform.position;
                    else globalTarget = lastNavPoint.GetRandomPoint(); 
                }
                else
                {
                    globalTarget = oppositeUnit.transform.position;
                }
            }
            else
            {
                globalTarget = lastNavPoint.GetRandomPoint();
            }

            if (globalTarget == null)
            {
                Debug.Log("GlobalTarget is null");
                return new Vector3();
            }
            
            return globalTarget;
        }

        public void UpdateOrientation()
        {
            GetOrientation();
            GetNavPoint();
        }

        public void GetOrientation()
        {
            if (orientation == AiOrientation.None)
            {
                orientation = AiOrientation.Left;
                return;
            }

            if (orientation == AiOrientation.Left)
            {
                orientation = AiOrientation.Right;
                return;
            }

            if (orientation == AiOrientation.Right)
            {
                orientation = AiOrientation.Left;
            }
        }

        private void GetNavPoint()
        {
            lastNavPoint = _navMap.GetNearPoint(orientation);
        }

        public void SetNavPoint(AiNavPoint point)
        {
            lastNavPoint = point;
        }
    }
}