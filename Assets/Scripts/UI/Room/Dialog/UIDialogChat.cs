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
    public class UIDialogChat : MonoBehaviour
    {
        public UiTransparency chatPanel;
        public TextMeshProUGUI chatText;
        private NpcCharacterDialog _npcCharacterDialog;
        public List<string> _characterPhrases;
        public int currentPhraseNumber;
        private UIDialogButtonGroup _buttonGroup;

        private void Awake()
        {
            _buttonGroup = GetComponent<UIDialogButtonGroup>();
        }

        public void SetCharacterDialog(NpcCharacterDialog dialog)
        {
            _npcCharacterDialog = dialog;
        }

        public void LoadPhrases(NpcCharacterProfile.Type type)
        {
            _characterPhrases = _npcCharacterDialog.GetCharacterPhrases(type);
        }

        private bool _finishChat;
        public void PlayPhrase()
        {
            if (currentPhraseNumber > _characterPhrases.Count - 1)
            {
                currentPhraseNumber = 0;
                _finishChat = true;
                ActiveChatPanel(false);
                return;
            }
            
            ActiveChatPanel(true);
            
            chatText.text = _characterPhrases[currentPhraseNumber];
            currentPhraseNumber++;
        }

        private void ActiveChatPanel(bool state)
        {
            Debug.Log("ActiveChatPanel " + state);

            if (chatPanel)
                chatPanel.Transparency(!state);   
            
            if (_buttonGroup) 
                _buttonGroup.ActiveButtonsPanel(!state);
            
            _npcCharacterDialog.npcCharacterCamera.PhraseCameraPose(state);
        }
    }
}

