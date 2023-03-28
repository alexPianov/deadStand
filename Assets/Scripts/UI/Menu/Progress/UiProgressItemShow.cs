using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiProgressItemShow : MonoBehaviour
    {
        public Transform instanceContainer;
        public Canvas itemCanvas;
        public Canvas seasonCanvas;
        public Camera itemCamera;
        private GameObject _spawnedPrefab;

        public void ItemPanel(bool state)
        {
            seasonCanvas.enabled = !state;
            itemCanvas.enabled = state;
            itemCamera.enabled = state;

            if (!state && _spawnedPrefab)
            {
                Destroy(_spawnedPrefab);
            } 
        }

        public async void OpenItem(Item instance)
        {
            if (instance == null)
            {
                ItemPanel(false);
                return;
            }

            ItemPanel(true);
            await CreateInstance(instance.info.itemName);
        }

        private async UniTask CreateInstance(string itemName)
        {
            if(!instanceContainer) return;
            
            if (_spawnedPrefab)
            {
                Destroy(_spawnedPrefab);
            }
            
            _spawnedPrefab = await AddressablesHandler.Get(itemName, instanceContainer);
            await GetComponent<ItemRotate>().SetObject(_spawnedPrefab);
        }
    }
}