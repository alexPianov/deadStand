using System;
using System.Threading.Tasks;
using CodeStage.AntiCheat.ObscuredTypes;
using Photon.Pun;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Playstel
{
    [RequireComponent(typeof(LineRenderer))]
    public class ItemFirearmLaserSight : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private Transform _startPos;
        private UnitFraction.Fraction currentFraction;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _startPos = transform;
            
            _layerMask = LayerMask.GetMask("Player", "Default", "Red", "Blue", "Collider");
            gameObject.layer = LayerMask.NameToLayer("UI");
            
            ActiveLaserSight(false);
            VisibleLine(true);
        }

        public void SetPlayerFraction(UnitFraction.Fraction fraction)
        {
            currentFraction = fraction;
        }

        private bool _enabled;
        public void ActiveLaserSight(bool state)
        {
            _enabled = state;

            if (_visibleLine) _lineRenderer.enabled = state;
            else _lineRenderer.enabled = false;
        }

        private bool _visibleLine;
        public void VisibleLine(bool state)
        {
            _visibleLine = state;
        }

        public void EditWidth(float value)
        {
            _lineRenderer.widthMultiplier = value;
            _lineRenderer.startWidth = value;
        }

        private Vector3 defaultVector = new Vector3(0,0,30);
        private RaycastHit _hit;
        private LayerMask _layerMask;
        private const int _maxDistance = 30;
        
        [SerializeField]
        private Collider targetCollider;
        
        public void Update()
        {
            if(!_enabled) return;
            
            if (Physics.Raycast(_startPos.position, _startPos.forward, out _hit, _maxDistance, _layerMask))
            {
                if (_hit.collider)
                {
                    targetCollider = _hit.collider;
                    _lineRenderer.SetPosition(1, new Vector3(0, 0, _hit.distance));
                }
            }
            else
            {
                _lineRenderer.SetPosition(1, defaultVector);
            }
        }

        public LayerMask GetColliderLayer()
        {
            if (targetCollider == null) return 0;
            return targetCollider.gameObject.layer;
        }
    }
}
