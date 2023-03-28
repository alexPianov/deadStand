using Lean.Gui;
using TMPro;
using UnityEngine;
using Zenject;

namespace Playstel
{
    [RequireComponent(typeof(LeanShake))]
    public class GuiResource : MonoBehaviour
    {
        private TextMeshProUGUI value;
        private LeanShake leanShake;

        [HideInInspector]
        [Inject] public Unit Unit;
        private int _shakePower = 10;
        private bool _shakeMode;

        private void Awake()
        {
            value = GetComponentInChildren<TextMeshProUGUI>();
            leanShake = GetComponent<LeanShake>();
        }

        public void ShakeMode(bool state)
        {
            _shakeMode = state;
        }

        public void EditValue(int amount)
        {
            value.text = amount.ToString();
            if(_shakeMode) leanShake.Shake(_shakePower);
        }
    }
}
