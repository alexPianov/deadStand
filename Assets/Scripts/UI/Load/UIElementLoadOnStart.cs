using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UIElementLoadOnStart : UIElementLoad
    {
        public UIElementContainer.Type elementType;

        [Inject] private UIElementContainer _elementsContainer;
        
        public void Start()
        {
            Load();
        }
        
        public async UniTask Load()
        {
            var element = await LoadElement(_elementsContainer.GetTransform());
            
            if(element == null) return;
            
            _elementsContainer.SetCurrentElement(element, elementType);
        }
    }
}
