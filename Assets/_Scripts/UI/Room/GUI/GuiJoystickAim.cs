using System;
using Lean.Gui;
using UnityEngine;
using Zenject;

namespace Playstel.UI
{
    [RequireComponent(typeof(LeanJoystick))]
    public class GuiJoystickAim : MonoBehaviour
    {
        public bool disableOnStart = true;

        public void Start()
        {
            gameObject.SetActive(!disableOnStart);
        }

        [Inject]
        private void Construct(Unit _unit)
        {
            var aimJoystick = GetComponent<LeanJoystick>();
            _unit.Joystick.SetAimJoystick(aimJoystick);
            _unit.GrenadeThrow.SetAimJoystick(aimJoystick);
        }
    }
}
