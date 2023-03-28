using System;
using Cysharp.Threading.Tasks;
using EventBusSystem;
using Lean.Gui;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel.UI
{
    [RequireComponent(typeof(LeanButton))]
    public class GuiButtonAttack : MonoBehaviour, IAttackHandler
    {
        public Image buttonImage;
        public Slider rateSlider;
        public Sprite highlightSprite;
        private Sprite startSprite;
        private LeanButton _leanButton;
        
        [Inject] 
        private Unit _unit;

        public bool disableOnStart = true;

        private void OnEnable()
        {
            EventBus.Subscribe(this);
        }
        
        private void OnDisable()
        {
            EventBus.Unsubscribe(this);
            HandleAttackSlider(false); 
        }

        private void Awake()
        {
            _leanButton = GetComponent<LeanButton>();
            
            UpdateSliderValues();
            SetStartSprite();
            BindButtonListener();
        }

        public void Start()
        {
            gameObject.SetActive(!disableOnStart);
        }

        private string keyboardButton = "f";
        public void Update()
        {
            if (Input.GetKeyDown(keyboardButton))
            {
                AttackOn();
            }

            if (Input.GetKeyUp(keyboardButton))
            {
                AttackOff();
            }
        }

        private void SetStartSprite()
        {
            startSprite = buttonImage.sprite;
        }

        private void BindButtonListener()
        {
            _leanButton.OnDown.AddListener(AttackOn);
            _leanButton.OnUp.AddListener(AttackOff);
        }

        private async void UpdateSliderValues()
        {
            await UniTask.WaitUntil(() => _unit.HandleItems.currentItemController);
            var itemController = _unit.HandleItems.currentItemController;
            await UniTask.WaitUntil(() => itemController.itemStat);
            HandleAttackSliderMaxValue(itemController.itemStat.attackRate);
            HandleAttackSliderValue(0);
            HandleAttackSlider(itemController.singleMode);
        }

        private void AttackOn()
        {
            HandleAttackMode(true);
        }

        private void AttackOff()
        {
            HandleAttackMode(false);
        }

        public void HandleAttackMode(bool state)
        {
            _unit.Aim.Attack(state);
            SetFocus(state);
        }

        public void HandleAttackSliderValue(float rate)
        { 
            rateSlider.value = rate;
            
            if (transparency) HandleAttackSlider(true);
            if (rate == 0) HandleAttackSlider(false); 
            if (rate >= rateSlider.maxValue) HandleAttackSlider(false); 
        }

        public void HandleAttackSliderMaxValue(float maxRate)
        {
            rateSlider.maxValue = maxRate;
        }

        private bool transparency;
        public void HandleAttackSlider(bool state)
        {
            transparency = !state;
            
            if (transparency)
            {
                rateSlider.targetGraphic.CrossFadeAlpha(1, 0, false);
                rateSlider.targetGraphic.CrossFadeAlpha(0, 0.2f, false);
            }
            else
            {
                rateSlider.targetGraphic.CrossFadeAlpha(0, 0, false);
                rateSlider.targetGraphic.CrossFadeAlpha(1, 0.2f, false);
            }
        }
        
        private void SetFocus(bool state)
        {
            buttonImage.sprite = state ? highlightSprite : startSprite;
        }
    }
}
