using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PlayFab.ClientModels;
using Playstel.UI;
using UnityEngine;
using Zenject;

namespace Playstel
{
    [RequireComponent(typeof(UiCharacterSaveMaster))]
    public class UiCharacterPick : MonoBehaviour
    {
        public UiCharacterSkinButtons SkinButtons;
        public bool disableLoadingScreen;
        
        private UiCharacterSaveMaster _uiCharacterSaveMaster;
        
        [Inject] private Unit _unit;

        private void Awake()
        {
            _uiCharacterSaveMaster = GetComponent<UiCharacterSaveMaster>();
        }

        public async void Pick(CatalogItem catalogItem)
        {
            if(DuplicateCall(catalogItem)) return;

            await UpdateCharacter(catalogItem);
            
            SetPickedCharacter(catalogItem);
            UpdateCustomizeButtons();
        }

        private bool DuplicateCall(CatalogItem _catalogItem)
        {
            return _uiCharacterSaveMaster.GetPickedCharacter() == _catalogItem;
        }

        private async UniTask UpdateCharacter(CatalogItem _catalogItem)
        {
            var schemeItems = _catalogItem.Bundle.BundledItems.ToArray();

            _unit.Builder.DisableLoadingScreen(disableLoadingScreen);
            
            await _unit.Builder.BuildUnit
                (schemeItems, null, ItemInfo.Class.Firearm, ItemInfo.Subclass.Autogun);
        }
        
        public void SetPickedCharacter(CatalogItem _catalogItem)
        {
            _uiCharacterSaveMaster.SetPickedCharacter(_catalogItem);
        }

        public void UpdateCustomizeButtons()
        {
            SkinButtons.UpdateCustomizeButtons();
        }
    }   
}
