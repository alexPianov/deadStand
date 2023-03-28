using Lean.Gui;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Playstel
{
    public class UiSoundMenuContext : MonoBehaviour, IPointerDownHandler
    {
        public CacheAudio.MenuSound MenuSound = CacheAudio.MenuSound.OnProcess;
        
        public CacheAudio CacheAudio;
        private bool playOverLast = true;
        
        private void Awake()
        {
            if(GetComponent<LeanButton>()) GetComponent<LeanButton>().OnClick.AddListener(PlaySound);
            
            if(GetComponent<LeanJoystick>()) GetComponent<LeanJoystick>().OnDown.AddListener(PlaySound);
            
            if(GetComponent<TMP_InputField>())
            {
                GetComponent<TMP_InputField>().onSelect
                    .AddListener(arg => PlaySound());
            }
            
            if(GetComponent<Toggle>()) 
            {
                playOverLast = false;
                
                GetComponent<Toggle>().onValueChanged
                    .AddListener(arg => PlaySound());
            }

            if (GetComponent<LeanToggle>())
            {
                playOverLast = false;
                
                GetComponent<LeanToggle>().OnOn.AddListener(PlaySound);
                GetComponent<LeanToggle>().OnOff.AddListener(PlaySound);
            }
            
            if (GetComponent<Slider>())
            {
                playOverLast = false;
                
                GetComponent<Slider>().onValueChanged
                    .AddListener(arg => PlaySound());
            }
            
            if (GetComponent<TMP_Dropdown>())
            {
                playOverLast = false;
                
                GetComponent<TMP_Dropdown>().onValueChanged
                    .AddListener(arg => PlaySound());
            }
        }

        private void PlaySound()
        {
            CacheAudio.Play(MenuSound, playOverLast);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(GetComponent<Toggle>()) return;
            PlaySound();
        }
    }
}