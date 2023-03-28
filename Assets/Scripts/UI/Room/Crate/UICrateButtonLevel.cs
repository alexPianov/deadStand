using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UICrateButtonLevel : GuiButtonInteraction
    {
        private UnitInteraction _cacheUnitInteraction;
        private Crate _crate;

        public void SetCrate(Crate crate)
        {
            _crate = crate;
        }
        
        public void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out UnitInteraction interaction))
            {
                if(PhotonNetwork.InRoom && !interaction.photonView.IsMine) return;
                
                _cacheUnitInteraction = interaction;
                _cacheUnitInteraction.SetCrate(_crate);
                base.ActiveButton(true);
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if(!_cacheUnitInteraction) return;
            
            if(PhotonNetwork.InRoom && !_cacheUnitInteraction.photonView.IsMine) return;
      
            _cacheUnitInteraction = null;
            base.ActiveButton(false);
        }
    }
}
