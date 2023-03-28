using System;
using CodeStage.AntiCheat.ObscuredTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiSettingsRegister : MonoBehaviour
    {
        public GameObject registerButtonBundle;
        public GameObject accountEmailBundle;
        public Button button;
        public GameObject registerPanel;
        public TextMeshProUGUI emailText;

        private void Awake()
        {
            button.onClick.AddListener(Register);
        }

        private void Register()
        {
            OpenRegisterPanel(true);
        }

        public void OpenRegisterPanel(bool state)
        {
            registerPanel.SetActive(state);
        }

        public void SetEmail(string email)
        {
            ActiveRegistedButton(email == null);
            emailText.text = "Register Email: " + email;
        }
        
        public void ActiveRegistedButton(bool state)
        {
            registerButtonBundle.SetActive(state);
            accountEmailBundle.SetActive(!state);
            emailText.enabled = !state;
        }
    }
}