using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Playstel
{
    public class UIDialogButtonGroup : MonoBehaviour
    {
        public UiTransparency buttonsPanel;
        public List<UIDialogButton> dialogButtons;

        private void Awake()
        {
            dialogButtons = buttonsPanel.GetComponentsInChildren<UIDialogButton>().ToList();
        }

        public void Initialize(NpcCharacterDialog dialog)
        {
            var buttonInfo = dialog.npcCharacter
                .GetCharacterData(NpcCharacter.DataKey.DialogButtons);
            
            if(string.IsNullOrEmpty(buttonInfo)) return;

            var buttonNames = DataHandler.SplitString(buttonInfo);

            foreach (var dialogButton in dialogButtons)
            {
                if (!dialogButton) continue;
                
                if (!buttonNames.Contains(dialogButton.currentType.ToString()))
                {
                    dialogButton.Disable(); continue;
                }

                if (dialogButton.currentType == UIDialogButton.ButtonType.Collects)
                {
                    UpdateCollectButton(dialog, dialogButton);
                }
                
                if (dialogButton.currentType == UIDialogButton.ButtonType.Token)
                {
                    UpdateTokenButton(dialogButton);
                }
            }
        }
 
        private void UpdateCollectButton(NpcCharacterDialog dialog, UIDialogButton dialogButton)
        {
            var collectItem = dialog.npcCharacter
                .GetCharacterData(NpcCharacter.DataKey.Collects);

            dialogButton.GetComponent<UIDialogButtonCollects>()
                .UpdateCollectItem(collectItem);
        }
        
        private void UpdateTokenButton(UIDialogButton dialogButton)
        {
            dialogButton.GetComponent<UIDialogButtonToken>().UpdateTokenDisplay();
        }

        public void ActiveButtonsPanel(bool state)
        {
            buttonsPanel.Transparency(!state);
        }
    }
}

