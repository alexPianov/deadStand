using Cysharp.Threading.Tasks;
using EventBusSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Playstel.UI
{
    public class GuiHandleItemBullets : MonoBehaviour, IAmmoHandler
    {
        public TextMeshProUGUI holdersCountText;
        public TextMeshProUGUI holderRemainText;
        public Image ammoImage;
        public Slider holderSlider;
        private ItemFirearm _firearm;

        private void OnEnable()
        {
            EventBus.Subscribe(this);
        }
        
        private void OnDisable()
        {
            EventBus.Unsubscribe(this);
        }

        public async UniTask SetFirearm(ItemFirearm firearm)
        {
            EventBus.Subscribe(this);
            _firearm = firearm;
            
            await UniTask.WaitUntil(() => _firearm.Ammo);
            
            ammoImage.sprite = _firearm.Ammo.GetAmmoInfo().itemSprite;

            SetDefaultSliderValues();
            _firearm.Ammo.Payload.UpdateBulletsUI();
        }

        private void SetDefaultSliderValues()
        {
            EditSliderValues(_firearm.Stat.maxBullets);
        }

        private void EditSliderValues(int value)
        {
            holderSlider.maxValue = value;
            holderSlider.value = value;
        }

        public void HandleHolderChange(int holderRemain, int holdersCount)
        {
            holderSlider.value = holderRemain;
            holderRemainText.text = holderRemain.ToString();

            if (holdersCount == 0)
            {
                holdersCountText.text = null;
            }
            else
            {
                holdersCountText.text = "x" + holdersCount;
            }
        }

        public void HandleHolderReload(float maxReloadTime)
        {
            holderSlider.maxValue = maxReloadTime;
            holderSlider.value = 0;
            reloading = true;
        }

        private bool reloading;
        private float waitingTime;
        private void Update()
        {
            if (reloading)
            {
                holderSlider.value += Time.deltaTime;

                if (holderSlider.value >= holderSlider.maxValue)
                {
                    reloading = false;
                    SetDefaultSliderValues();
                }
            }
        }
    }
}
