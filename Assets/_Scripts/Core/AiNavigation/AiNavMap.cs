using System;
using System.Collections.Generic;
using System.Linq;
using Playstel;
using UnityEngine;
using Zenject;
using static Playstel.UnitAiTargetGlobal;
using Random = UnityEngine.Random;

namespace Core.AiNavigation
{
    public class AiNavMap : MonoBehaviour
    {
        [SerializeField] private List<AiNavPoint> points = new ();
        [Inject] public LocationInstaller _locationInstaller;

        private void Awake()
        {
            _locationInstaller.BindFromInstance(this);
        }

        public AiNavPoint GetNearPoint(AiOrientation orientation)
        {
            return points.Find(point => point.Orientation == orientation);
        }
    }
}