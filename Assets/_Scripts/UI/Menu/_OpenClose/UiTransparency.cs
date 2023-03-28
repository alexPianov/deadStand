using System;
using UnityEngine;

namespace Playstel
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UiTransparency : MonoBehaviour
    {
        [HideInInspector]
        public CanvasGroup сanvasGroup;
        
        public bool enableAtStart;
        
        private void OnEnable()
        {
            GetCanvasGroup();
            
            сanvasGroup.alpha = 0;
            
            Transparency(enableAtStart);
        }

        private void GetCanvasGroup()
        {
            if (сanvasGroup) return;
            
            if (!GetComponent<CanvasGroup>())
            {
                сanvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            else
            {
                сanvasGroup = GetComponent<CanvasGroup>();
            }
        }

        public void Enable(bool state)
        {
            if(сanvasGroup) сanvasGroup.enabled = state;
        }

        public void BlocksRaycasts(bool state)
        {
            if(сanvasGroup) сanvasGroup.blocksRaycasts = state;
        }
        
        private bool _transparency;
        public void Transparency(bool state)
        {
            if(!сanvasGroup) return;
            _transparency = state;
            BlocksRaycasts(!state);
        }
        
        private const float transparenceTime = 6f;
        
        private void Update()
        {
            if (!сanvasGroup)
            {
                Debug.Log("Canvas Group is null");return;
            }

            if (_transparency)
            {
                if(сanvasGroup.alpha <= 0) return;
                сanvasGroup.alpha -= Time.deltaTime * transparenceTime;
                if (сanvasGroup.alpha <= 0.1f) сanvasGroup.alpha = 0;
            }
            else
            {
                if(сanvasGroup.alpha >= 1) return;
                сanvasGroup.alpha += Time.deltaTime * transparenceTime;
                if (сanvasGroup.alpha >= 1) сanvasGroup.alpha = 1; 
            }
        }
    }
}
