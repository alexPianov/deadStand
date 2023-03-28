using System.Collections.Generic;
using System.Linq;
using Playstel.UI;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiCharacterLoad : MonoBehaviour
    {
        public UiCharacterNickname UiCharacterNickname;
        public UiCharacterSkinButtons UiCharacterSkinButtons;
        
        [Inject] private Unit _unit;
        [Inject] private CacheUserInfo _cacheUserInfo;

        public void LoadCharacterComponents(List<UiCharacterSlot> characterSlots)
        {
            FindCurrentCharacterSlot(characterSlots);
            SetCharacerNicknameToInputHolder();
            UiCharacterSkinButtons.UpdateCustomizeButtons();
        }

        private void FindCurrentCharacterSlot(List<UiCharacterSlot> characterSlots)
        {
            var rigName = _unit.Skin.currentRigName;

            foreach (var slot in characterSlots)
            {
                var bundledItems = slot.GetCatalogItem().Bundle.BundledItems;
                
                if (bundledItems.Contains(rigName))
                {
                    slot.SetSlotAsPickedAtStart(); break;
                } 
            }
        }

        private void SetCharacerNicknameToInputHolder()
        {
            if(!UiCharacterNickname) return;
            
            var nickname = _cacheUserInfo.payload.GetTitleDisplayName();
            UiCharacterNickname.SetCurrentNickname(nickname);
        }
    }
}
