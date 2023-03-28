using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Playstel
{
    [RequireComponent(typeof(NpcCharacter))]
    public class NpcCharacterDialog : MonoBehaviour
    {
        [HideInInspector]
        public NpcCharacter npcCharacter;
        
        [HideInInspector]
        public NpcCharacterCamera npcCharacterCamera;
        private NpcCharacterProfile _npcCharacterProfile;

        private void Awake()
        {
            npcCharacter = GetComponent<NpcCharacter>();
            npcCharacterCamera = GetComponent<NpcCharacterCamera>();
        }

        public void SetDialogProfile(NpcCharacterProfile characterProfile)
        {
            _npcCharacterProfile = characterProfile;
            SetButtonTrigger();
        }
        
        private void SetButtonTrigger()
        {
            GetComponentInChildren<UIDialogButtonLevel>().SetNpcCharacterDialog(this);
        }

        public List<string> GetCharacterPhrases(NpcCharacterProfile.Type type)
        {
            return _npcCharacterProfile.GetProfilePhrases(type);
        }

        public UnitFraction.Fraction GetCharacterFraction()
        {
            return _npcCharacterProfile.characterFraction;
        }
    }
}
