using UnityEngine;
using UnityEngine.UI;

namespace Playstel
{
    public class UiPolicies : MonoBehaviour
    {
        [Header("Confirm Button")]
        public Button confirmButton;

        [Header("Policy Buttons")] 
        public Button termsButton;
        public Button policyButton;

        private void Start()
        {
            termsButton.onClick.AddListener(ReadTerms);
            policyButton.onClick.AddListener(ReadPrivatePolicy);
        }

        public void ReadTerms()
        {
            Application.OpenURL(KeyStore.TERMS_REF);
        }
        
        public void ReadPrivatePolicy()
        {
            Application.OpenURL(KeyStore.PRIVATE_POLICY_REF);
        }

        public void Checkbox(Toggle toggle)
        {
            confirmButton.interactable = toggle.isOn;
        }
    }
}
