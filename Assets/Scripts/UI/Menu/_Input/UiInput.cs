using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Lean.Gui;
using UnityEngine;
using Zenject;

namespace Playstel
{
    [RequireComponent(typeof(TMP_InputField))]
    public class UiInput : MonoBehaviour
    {
        [Header("Limits")]
        public int InputMin = 6;
        public int InputMax = 30;
        const int guidLenght = 16;
        public bool isNickname;

        [HideInInspector] public TMP_InputField inputField;

        private List<string> forbiddenSymbols = new()
        {
            ";", "'", "/", "`", "~", "±", "§", "#", "+",
            "%", "^", "&", "*", "(", ")", "=", "<",
            ":", "[", "]", "{", "}", "|", ",", "?", ">",
            "!", "©", "®", "≤", "≥", "§"
        };
        
        private List<string> spaceSymbols = new()
        {
            " ", "  ", "   ", "    ", "     "
        };
        
        private List<string> forbiddenSymbolsUsername = new()
        {
            "!", "@", "-"
        };

        [Inject] private HandlerPulse _handlerPulse;
        
        public void Awake()
        {
            inputField = GetComponent<TMP_InputField>();
        }

        public void Clear()
        {
            if (inputField) inputField.text = null;
        }

        public string GetText()
        {
            var text = inputField.text;
            
            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text) )
            {
                _handlerPulse.OpenTextNote("Input word is empty");
                return null;
            }
            
            if (text.Length > InputMax)
            {
                _handlerPulse.OpenTextNote("Input length must be smaller than " + InputMax + " chars");
                return null;
            }
            
            if (text.Length < InputMin)
            {
                _handlerPulse.OpenTextNote("Input length must be longer than " + InputMin + " chars");
                return null;
            }

            foreach (var symbol in forbiddenSymbols)
            {
                if (text.Contains(symbol))
                {
                    _handlerPulse.OpenTextNote("Forbidden symbol: " + symbol);
                    return null;
                }
            }
            
            foreach (var symbol in spaceSymbols)
            {
                if (text.Contains(symbol))
                {
                    _handlerPulse.OpenTextNote("Input contains spaces between chars");
                    return null;
                }
            }

            if (isNickname)
            {
                foreach (var symbol in forbiddenSymbolsUsername)
                {
                    if (text.Contains(symbol))
                    {
                        _handlerPulse.OpenTextNote("Forbidden symbol: " + symbol);
                        return null;
                    }
                }
            }

            return text;
        }

        public string GetTextGuid()
        {
            var text = inputField.text;

            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text) )
            {
                _handlerPulse.OpenTextNote("Input word is empty");
                return null;
            }

            if (text.Length != guidLenght)
            {
                _handlerPulse.OpenTextNote("Code length must be " + guidLenght + " chars");
                return null;
            }
            
            return text;
        }
    }
}
