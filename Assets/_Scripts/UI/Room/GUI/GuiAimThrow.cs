using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Gui;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel.UI
{
    public class GuiAimThrow : MonoBehaviour
    {
        public GameObject throwAimTarget;
        public LineRenderer lineRenderer;
        
        [Inject]
        private void Construct(Unit _unit)
        {
            _unit.GrenadeThrow.SetAim(this);
            _unit.Joystick.SetJoysticTarget(transform);
        }
    }
}
