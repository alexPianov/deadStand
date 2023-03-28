using Lean.Gui;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel.UI
{
    [RequireComponent(typeof(LeanJoystick))]
    public class GuiJoystickRun : MonoBehaviour
    {
        [Inject]
        private void Construct(Unit _unit)
        {
            var runJoystick = GetComponent<LeanJoystick>();
            
            _unit.Move.SetRunJoystick(runJoystick);
            _unit.Joystick.SetRunJoystick(runJoystick);
        }
    }
}
