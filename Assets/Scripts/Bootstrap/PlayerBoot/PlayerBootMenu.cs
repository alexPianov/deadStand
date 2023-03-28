using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using PlayFab.Json;
using UnityEngine;
using Zenject;

namespace Playstel
{
    [RequireComponent(typeof(UIElementLoadByType))]
    public class PlayerBootMenu : MonoBehaviour
    {
        [Header("Unit")]
        public GameObject playerPrefab;
        public Transform spawnPoint;
        
        [Header("Spawn Mode")]
        public bool setItems;
        public bool disableLoadingScreen;
        public bool spawnOnStart;
        public int spawnDelay;
        
        [Header("Default Ui")]
        public UIElementLoad.Elements defaultUiElement = UIElementLoad.Elements.MainMenu;
        
        [Header("Weapon")] public ItemInfo.Subclass startWeaponSubClass 
            = ItemInfo.Subclass.Autogun;

        [Inject]
        private LocationInstaller _locationInstaller;

        [Inject] private HandlerLoading _handlerLoading;

        private ItemRotate _itemRotate;

        private void Awake()
        {
            _itemRotate = GetComponent<ItemRotate>();
        }

        private void Start()
        {
            if(spawnOnStart) SpawnUnitInMenu(defaultUiElement);
        }

        public async void SpawnUnitInMenu(UIElementLoadByType.Elements element)
        {
            var instance = Instantiate
                (playerPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint.parent);

            if (instance.TryGetComponent(out Unit unit))
            {
                _locationInstaller.BindFromInstance(unit);

                SetUnitMenuTransform(instance.transform);
                SetUnitMenuConstrains(instance);
                SetBuildMode(unit);
                await SetUnitItems(unit);
                SetUnitLocalPosition(instance);
                await SetUnitMenuView(instance);
                await LoadUi(element);
                
                _handlerLoading.OpenBlackScreen(false);
            }
        }

        private async UniTask LoadUi(UIElementLoad.Elements elements)
        {
            if(!this) return;
            await GetComponent<UIElementLoadByType>().Load(elements, UIElementContainer.Type.Screen);
        }

        private static void SetUnitLocalPosition(GameObject unit)
        {
            if(unit) unit.transform.localPosition = new Vector3(0, 0, 0);
        }

        private async UniTask SetUnitMenuView(GameObject unit)
        {
            await _itemRotate.SetObject(unit, spawnDelay);
        }

        private async Task SetUnitItems(Unit unit)
        {
            if (setItems)
            {
                var sessionItems = await GetSessionItems();

                if (sessionItems == null)
                {
                    Debug.Log("Failed to find Session Items | Reset game");
                    SceneHandler.LoadAsync(SceneHandler.Scenes.Boot);
                    return;
                }

                var customMaterial = unit.Builder.GetCustomMaterial();
                
                await unit.Builder.BuildUnit(sessionItems, customMaterial, 
                    ItemInfo.Class.Firearm, startWeaponSubClass);
            }
        }

        private async UniTask<string[]> GetSessionItems()
        {
            var result = await PlayFabHandler.ExecuteCloudScript
                (PlayFabHandler.Function.GetSessionItems);

            if (result == null) return null;
            if (result.FunctionResult == null) return null;

            return PlayFabSimpleJson
                .DeserializeObject<string[]>(result.FunctionResult.ToString());
        }
        
        private void SetBuildMode(Unit unit)
        {
            unit.Builder.CreateItemsData(false);
            unit.Builder.DisableLoadingScreen(disableLoadingScreen);
        }

        private void SetUnitMenuTransform(Transform unitTransform)
        {
            unitTransform.SetParent(spawnPoint);
            unitTransform.rotation = spawnPoint.rotation;
            unitTransform.position = spawnPoint.position;
        }

        private static void SetUnitMenuConstrains(GameObject unit)
        {
            unit.GetComponent<Rigidbody>().isKinematic = true;
            unit.transform.localPosition = new Vector3(0, -100, 0);
        }
    }
}
