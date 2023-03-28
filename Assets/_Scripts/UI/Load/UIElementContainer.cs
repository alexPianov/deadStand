using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Playstel
{
    public class UIElementContainer : MonoBehaviour
    {
        private Transform _transform;
        private Dictionary<Type, GameObject> cacheElements = new ();
        private GameObject clipboardObject;
        private Dictionary<ClipboardStringType, string> clipboardStrings = new ();
        
        public enum Type
        {
            Screen, Popup
        }

        private void Awake()
        {
            _transform = transform;
        }
        
        public Transform GetTransform()
        {
            return _transform;
        }

        public void SetCurrentElement(GameObject element, Type type)
        {
            if (cacheElements.ContainsKey(type))
            {
                DestroyCurrentElement(type);
            }

            cacheElements.Add(type, element);
        }

        public GameObject GetCurrentElement(Type type)
        {
            cacheElements.TryGetValue(type, out var value);
            return value;
        }
        
        public void DestroyCurrentElement(Type type)
        {
            if (cacheElements.TryGetValue(type, out var element))
            {
                cacheElements.Remove(type);
                Destroy(element);
            }
        }
        
        public void SetClipboardString(ClipboardStringType type, string stringData)
        {
            if (clipboardStrings.ContainsKey(type))
            {
                clipboardStrings.Remove(type);
            }
            
            clipboardStrings.Add(type, stringData);
        }

        public enum ClipboardStringType
        {
            FriendId
        }
        
        public string GetClipboardString(ClipboardStringType type)
        {
            clipboardStrings.TryGetValue(type, out var value);
            return value;
        }
    }
}