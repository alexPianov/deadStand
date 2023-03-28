using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PlayFab.ClientModels;
using Playstel.UI;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiCharacterSkinButtons : MonoBehaviour
    {
        private List<UiCharacterSkinColorButton> SkinColorButtons = new();

        public UiCharacterSkinClothesButton SkinClothesButton;
        public Transform skinColotButtonsLayout;

        [Inject] private Unit _unit;
        
        private void Awake()
        {
            SkinColorButtons = skinColotButtonsLayout
                .GetComponentsInChildren<UiCharacterSkinColorButton>().ToList();
        }
        
        public void UpdateCustomizeButtons()
        {
            SkinClothesButton.SetupClothesColor();
            SetupSkinColorButtons();
        }
        
        private void SetupSkinColorButtons()
        {
            foreach (var colorButton in SkinColorButtons)
            {
                var result = colorButton
                    .currentType.ToString() == _unit.Skin.currentMaterialType;

                colorButton.ActiveToggle(result);
            }
        }
    }   
}
