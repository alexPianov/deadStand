using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UnitItemSpawner : MonoBehaviourPun
    {
        [Inject] private LocationInstaller _locationInstaller;
        
        private Unit _unit;
        public void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        public async UniTask SpawnItem(Item item)
        {
            var rootPart = _unit.Root.GetRootPart(item.info);
            
            var instance = await _locationInstaller.LoadElement<GameObject>
            (item.info.itemName, rootPart);

            if (!instance) return;
            
            item.SetItemInstance(instance);
            
            await AddItemComponents(item);
            
            SetMaterial(item);
            
            _unit.HandleItems.Add(item);

            if (item.info.itemClass == ItemInfo.Class.Firearm)
            {
                _unit.HandleItems.PickItem(item, true);
            }
        }

        private async UniTask AddItemComponents(Item item)
        {
            var instance = item.instance;

            instance.name = item.info.itemName;

            var renderer = instance.AddComponent<ItemRenderer>();

            if (!_unit.Renderer.renderIsActive)
            {   
                renderer.Active(false);
            }

            if (item.info.itemCatalog == ItemInfo.Catalog.Character) return;
            if (_unit.Builder.DontCreateItemData) return;
            if (GetComponent<NpcCharacter>()) return;

            instance.AddComponent<ItemStat>().SetInfo(item.info);
            instance.AddComponent<ItemController>().Initialize(item, _unit);

            if (item.info.itemClass == ItemInfo.Class.Firearm)
            {
                await GetComponent<UnitFirearmBoot>().Boot(item, instance);
            }

            if (item.info.itemClass == ItemInfo.Class.Melee)
            {
                instance.AddComponent<ItemMelee>().SetComponents(item, _unit);
            }

            if (item.info.itemClass == ItemInfo.Class.Grenade)
            {
                instance.GetComponent<ItemGrenade>().SetComponents(item, _unit);
            }
        }

        private async void SetMaterial(Item item)
        {
            var materialName = item.info.GetMaterialName();
            var material = await AddressablesHandler.Load<Material>(materialName);

            if (item.instance == null)
            {
                Debug.Log("Item instance is null for: " + item.info.itemName);
                return;
            }
            
            item.instance.GetComponent<MeshRenderer>().material = material;

            var renderers = item.instance.transform.GetComponentsInChildren<MeshRenderer>();

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material = material;
            }
        }

    }
}
