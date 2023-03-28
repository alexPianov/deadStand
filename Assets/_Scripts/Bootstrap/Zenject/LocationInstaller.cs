using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Playstel
{
    public class LocationInstaller : MonoInstaller
    {
        [Header("Bind Elements")] 
        public UIElementContainer uiElementContainer;
        
        public override void InstallBindings()
        {
            BindFromInstance(this);
            BindFromInstance(uiElementContainer);
        }

        public void BindFromInstance<T>(T instance)
        {
            if (Container.HasBinding<T>())
            {
                Container.Unbind<T>();
            }

            Container.Bind<T>().FromInstance(instance).AsTransient();
        }

        public async UniTask<GameObject> LoadElement<T>(string objectName, Transform parent = null)
        {
            await LoadAssetAsync<T>(objectName);

            var instance = await Addressables.InstantiateAsync(objectName, parent);
            
            RemoveFromMemoryOnDestroy<T>(instance);

            BindToSceneContext<T>(instance);

            return instance;
        }
        
        private static void RemoveFromMemoryOnDestroy<T>(GameObject instance)
        {
            AddressablesHandler.ReleaseFromMemoryOnDestroy(instance);
        }

        private void BindToSceneContext<T>(GameObject instance)
        {
            Container.InstantiateComponent<ZenAutoInjecter>(instance)
                .ContainerSource = ZenAutoInjecter.ContainerSources.SceneContext;
        }

        private Object _loadedResource;
        private async Task LoadAssetAsync<T>(string objectName)
        {
            if (_loadedResource == null || _loadedResource.name != objectName)
            {
                _loadedResource = await Addressables.LoadAssetAsync<T>(objectName) as Object;
            
                if (_loadedResource == null) Debug.LogError("LoadedResource is null"); 
            }
        }
        
        public async UniTask<TContract> InstantiateComponent<TContract>(GameObject instance) where TContract : Component
        {
            return Container.InstantiateComponent<TContract>(instance);
        }
    }
}