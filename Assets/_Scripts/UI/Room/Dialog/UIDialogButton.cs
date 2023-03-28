using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UIDialogButton : MonoBehaviour
    {
        public ButtonType currentType;
        
        public enum ButtonType
        {
            Info,Barter,Weapons,Support,Casino,Collects,Token
        }

        public void Disable()
        {
            if(gameObject) gameObject.SetActive(false);
        }
    }
}

