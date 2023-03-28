using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UIElementLoadByType : UIElementLoad
    {
        public UIElementContainer.Type elementType;

        [Inject] private UIElementContainer _elementsContainer;

        public async UniTask Load(Elements element, UIElementContainer.Type type)
        {
            openingElement = element;
            elementType = type;

            await Load();
        }

        public async UniTask Load()
        {
            if (elementType == UIElementContainer.Type.Screen)
            {
                if (PopupIsOpened())
                {
                    _elementsContainer.DestroyCurrentElement(UIElementContainer.Type.Popup);
                    return;
                }

                _elementsContainer.DestroyCurrentElement(UIElementContainer.Type.Screen);
                
                LoadElement();
            }
            
            if (elementType == UIElementContainer.Type.Popup)
            {
                _elementsContainer.DestroyCurrentElement(UIElementContainer.Type.Popup);
                
                LoadElement();
            }
        }

        public async UniTask LoadElement()
        {
            var newElement = await LoadElement(_elementsContainer.GetTransform());

            _elementsContainer.SetCurrentElement(newElement, elementType);
        }
        
        private bool PopupIsOpened()
        {
            return _elementsContainer.GetCurrentElement(UIElementContainer.Type.Popup);
        }
    }
}
