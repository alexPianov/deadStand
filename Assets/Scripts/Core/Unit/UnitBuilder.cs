using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Playstel
{
    [RequireComponent(typeof(UnitItemSpawner), typeof(UnitItems))]
    public class UnitBuilder : MonoBehaviourPun
    {
        [HideInInspector] public bool DontCreateItemData;
        [HideInInspector] public bool EnableLoadingScreen;
        [HideInInspector] public bool IsBuild;
        
        [Inject] 
        private HandlerLoading _handlerLoading;
        
        private string[] mineItems;
        private string _customSkinMaterial;

        private Unit _unit;
        
        private void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        public async UniTask BuildUnit(string[] itemNames, string customSkinMaterial = null,
            ItemInfo.Class primaryClass = ItemInfo.Class.Null, 
            ItemInfo.Subclass primarySubclass = ItemInfo.Subclass.Null, 
            bool isMine = true)
        {
            SetCustomMaterial(customSkinMaterial);

            string unitItemNames = null;

            foreach (var itemName in itemNames)
            {
                unitItemNames += itemName + ";";
            }
            
            if (isMine)
            {
                mineItems = itemNames;
            }

            var items = _unit.CacheItemInfo.CreateItemList(itemNames);
                
            if (primaryClass != ItemInfo.Class.Null)
            {
                RandomItem(items, primaryClass, primarySubclass);
            }

            if(EnableLoadingScreen) _handlerLoading.OpenLoadingScreen(true, HandlerLoading.Text.Creating);

            if(!_returningMineUnit) _unit.Renderer.Active(false);
            
            _unit.Items.RemoveInventoryItems();
            
            await AddItems(items); 
            
            if(!_returningMineUnit) _unit.Renderer.Active(true);

            await UniTask.Delay(200);
            
            if(EnableLoadingScreen) _handlerLoading.OpenLoadingScreen(false);
            
            IsBuild = true;
        }

        public void SetCustomMaterial(string materialName)
        {
            _customSkinMaterial = materialName;
        }
        
        public void DisableLoadingScreen(bool state = true)
        {
            EnableLoadingScreen = !state;
        }
        
        public void CreateItemsData(bool state = true)
        {
            DontCreateItemData = !state;
        }

        private void RandomItem(List<Item> items, ItemInfo.Class primaryClass, 
            ItemInfo.Subclass primarySubclass)
        {
            items.Add(GetRandomItem(ItemInfo.Catalog.Weapons, primaryClass, primarySubclass));
        }
        
        public async UniTask AddItems(List<Item> items)
        {
            foreach (var item in items)
            {
                await AddItem(item);
            }
        }

        public async UniTask AddItem(ItemInfo itemInfo)
        {
            AddItem(ItemHandler.CreateItem(itemInfo));
        }

        public async UniTask AddItem(Item item)
        {
            if (item == null) return;
            
            if (_unit.Items.HaveSameItem(item) && PhotonNetwork.InRoom)
            {
                Debug.Log("The unit already has an item: " + item.info.itemName); return;
            }
            
            if (item.info.itemCatalog == ItemInfo.Catalog.Character)
            {
                _unit.Items.RemoveSameSubClassItem(item);
                
                if (item.info.itemClass == ItemInfo.Class.Rig)
                {
                    if (_unit.Skin) await _unit.Skin.SetRigComponents(item, _customSkinMaterial);
                    if (_unit.NpcSkin) await _unit.NpcSkin.SetRigComponents(item);

                    if (PhotonNetwork.InRoom)
                    {
                        if(_unit.SFX) _unit.SFX.SetUnitSFX(item.info);
                        if(_unit.VFX) _unit.VFX.SetUnitVFX(item.info);
                    }
                }
                
                if (item.info.itemClass == ItemInfo.Class.Stuff)
                {
                    await _unit.ItemSpawner.SpawnItem(item);
                }

                _unit.Items.AddItem(item);
                _unit.Items.RemoveBeardBlockingItems();
            }
            
            if (item.info.itemCatalog == ItemInfo.Catalog.Weapons)
            {
                _unit.Items.RemoveSameClassItem(item);
                await _unit.ItemSpawner.SpawnItem(item);
                _unit.Items.AddItem(item);
            }
            
            if (item.info.itemCatalog == ItemInfo.Catalog.Support)
            {
                if (item.info.itemClass == ItemInfo.Class.Ammo)
                {
                    _unit.Ammo.Add(item); return;
                }

                _unit.Items.AddItem(item);
            }
        }

        public bool IsAlreadyAdd(ItemInfo itemInfo)
        {
            var result = _unit.Items.IsAlreadyExists(itemInfo);
            
            if (result) _unit.Items.RemoveItem(result, false);

            return result;
        }

        public Item GetRandomItem(ItemInfo.Catalog itemCatalog, 
            ItemInfo.Class itemClass, ItemInfo.Subclass itemSubclass)
        {
            var items = _unit.CacheItemInfo.GetItemInfoList
                (itemCatalog, itemClass, itemSubclass);

            return GetRandomItemFromInfoList(items);
        }

        private Item GetRandomItemFromInfoList(List<ItemInfo> list)
        {
            var firearmItemInfo = list[Random.Range(0, list.Count)];
            return ItemHandler.CreateItem(firearmItemInfo);
        }

        private bool _returningMineUnit;
        public async void ReturtMineSetup()
        {
            _returningMineUnit = true;
            await BuildUnit(mineItems, GetCustomMaterial(), ItemInfo.Class.Firearm, ItemInfo.Subclass.Autogun);
            _returningMineUnit = false;
        }

        public string GetCustomMaterial()
        {
            return _unit.CacheUserInfo.data.GetUserData(UserData.UserDataType.UnitSkin);
        }
    }
}
