using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UIDialogButtonLevel : GuiButtonInteraction
    {
        private UnitInteraction _cacheUnitInteraction;
        private NpcCharacterDialog npcCharacterDialog;
        
        public void SetNpcCharacterDialog(NpcCharacterDialog _npcCharacterDialog)
        {
            npcCharacterDialog = _npcCharacterDialog;
        }
        
        public void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Unit unit))
            {
                if (PhotonNetwork.InRoom && !unit.photonView.IsMine) return;
                
                if (unit.IsNPC) return;
                
                if (!unit.Interaction) return;
                
                if (npcCharacterDialog.GetCharacterFraction() != UnitFraction.Fraction.Null)
                {
                    if (npcCharacterDialog.GetCharacterFraction() != unit.Fraction.currentFraction) 
                        return; 
                }

                _cacheUnitInteraction = unit.Interaction;
                _cacheUnitInteraction.SetDialogNPC(npcCharacterDialog);
                ActiveButton(true);
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Unit unit))
            {
                if (PhotonNetwork.InRoom && !unit.photonView.IsMine) return;
                
                if (unit.IsNPC) return;

                _cacheUnitInteraction = null;
                ActiveButton(false);
            }
        }
    }
}
