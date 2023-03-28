using Cysharp.Threading.Tasks;
using Lean.Gui;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel.UI
{
    public class UiCharacterSkinColorButton : MonoBehaviour
    {
        [Inject] private Unit _unit;
        
        public Type currentType;
        public UiCharacterSkinClothesButton SkinClothesButton;
        private LeanButton _leanButton; 

        public enum Type
        {
            Light, Dark, Yellow
        }
        
        public void Start()
        {
            _leanButton = GetComponent<LeanButton>();
            
            GetComponent<LeanToggle>().OnOn.AddListener(Action);
            GetComponent<LeanToggle>().OnOff.AddListener(UnblockToggle);
        }

        private void Action()
        {
            _unit.Skin.currentMaterialType = currentType.ToString();
            
            _unit.Skin.skinMaterialNames
                .TryGetValue(currentType.ToString(), out var names);

            var clothesColor = SkinClothesButton.currentNumber;
            
            _unit.Skin.SelectSkinFromMaterials(names[clothesColor]);

            _leanButton.interactable = false;
        }

        private void UnblockToggle()
        {
            _leanButton.interactable = true;
        }

        public void ActiveToggle(bool state)
        {
            GetComponent<LeanToggle>().On = state;
        }
    }
}
