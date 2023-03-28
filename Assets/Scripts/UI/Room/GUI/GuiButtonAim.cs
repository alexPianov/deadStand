using Cysharp.Threading.Tasks;
using EventBusSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel.UI
{
    [RequireComponent(typeof(Button))]
    public class GuiButtonAim : MonoBehaviour, IAimingPermissionHandler
    {
        [Inject]
        private Unit _unit;

        [Header("Focus")]
        public Image iconFocus;
        
        [Header("Refs")]
        public GameObject joystickAim;
        public GameObject buttonAttack;
        public GameObject buttonAttackSecond;
        public GameObject buttonSprint;

        [Inject] private CacheAudio _cacheAudio;

        private Button _button;

        public void Awake()
        {
            AddListener();
        }
        
        private async void OnEnable()
        {
            EventBus.Subscribe(this);

            await UniTask.WaitUntil(() => _unit.AreaBehaviour);
            
            _unit.AreaBehaviour.RefreshCurrentAimingPermission();
            
            ActiveAimUi(_unit.Aim.aiming);
        }
        
        private void OnDisable()
        {
            EventBus.Unsubscribe(this);
        }

        private void AddListener()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Switch);
        }

        private void Switch()
        {
            if (_unit.Sprint.sprint) return;
            
            _unit.Aim.Aiming(!_unit.Aim.aiming);
            AimSound(_unit.Aim.aiming);
            
            ActiveAimUi(_unit.Aim.aiming);
        }

        private void ActiveAimUi(bool state)
        {
            SetFocus(state);

            joystickAim.SetActive(state);
            buttonAttack.SetActive(state);
            buttonAttackSecond.SetActive(state);
            buttonSprint.SetActive(!state);
        }

        private void SetFocus(bool state)
        {
            if (iconFocus) iconFocus.enabled = state;
        }

        private void AimSound(bool state)
        {
            if (state)
            {
                _cacheAudio.Play(CacheAudio.MenuSound.OnAimOn);
            }
            else
            {
                _cacheAudio.Play(CacheAudio.MenuSound.OnAimOff);
            }
        }

        public void HandlerAimingPermission(bool state)
        {
            _button.interactable = state;
            
            if (!state) ActiveAimUi(false); 
        }
    }
}
