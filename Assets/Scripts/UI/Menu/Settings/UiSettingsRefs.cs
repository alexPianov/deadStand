using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiSettingsRefs : MonoBehaviour
    {
        public void OpenSupport()
        {
            Application.OpenURL(KeyStore.SUPPORT_REF);
        }

        public void OpenPolicy()
        {
            Application.OpenURL(KeyStore.PRIVATE_POLICY_REF);
        }
        
        public void OpenTerms()
        {
            Application.OpenURL(KeyStore.TERMS_REF);
        }

        public void OpenRateApp()
        {
            Application.OpenURL(KeyStore.RATE_REF);
        }
        
        public void OpenSite()
        {
            Application.OpenURL(KeyStore.SITE_REF);
        }

        public void OpenVersions()
        {
            Application.OpenURL(KeyStore.VERSIONS_REF);
        }
    }
}