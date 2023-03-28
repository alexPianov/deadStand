using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UIElementLoadOnBoot : MonoBehaviour
    {
        [Inject]
        private LocationInstaller _installer;

        [Inject]
        private UIElementContainer _uiElementsContainer;

        public async UniTask Load(UIElementLoad.Elements loadElement, UIElementContainer.Type type, bool dontSaveInCache = false)
        {
            var elementName = loadElement.ToString();
            
            var element = await _installer.LoadElement<GameObject>(elementName, transform);
            
            if (element == null)
            {
                Debug.Log("Failed to load element: " + elementName); return;
            }

            if (dontSaveInCache) return;
            
            _uiElementsContainer.SetCurrentElement(element, type);
        }
    }
}
