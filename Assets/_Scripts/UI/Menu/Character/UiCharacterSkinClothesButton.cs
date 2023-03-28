using Cysharp.Threading.Tasks;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel.UI
{
    [RequireComponent(typeof(Button))]
    public class UiCharacterSkinClothesButton : MonoBehaviour
    {
        [Inject] private Unit _unit;
        
        public int currentNumber;
        
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(Action);
        }

        private void Action()
        {
            _unit.Skin.skinMaterialNames
                .TryGetValue(_unit.Skin.currentMaterialType, out var names);

            currentNumber++;
            
            if (currentNumber > names.Count - 1)
            {
                currentNumber = 0;
            }
            
            _unit.Skin.SelectSkinFromMaterials(names[currentNumber]);
        }

        public void SetupClothesColor()
        {
            _unit.Skin.skinMaterialNames
                .TryGetValue(_unit.Skin.currentMaterialType, out var names);

            for (int i = 0; i < names.Count; i++)
            {
                if (names[i] == _unit.Skin.currentMaterialName)
                {
                    currentNumber = i;
                }
            }
        }
    }
}
