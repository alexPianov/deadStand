using CodeStage.AntiCheat.ObscuredTypes;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    [CreateAssetMenu(menuName = "Item/Info")]
    public class ItemInfo : ScriptableObject
    {
        [Header("Info")] 
        public ObscuredString itemName, itemId;
        public Sprite itemSprite;
        public Catalog itemCatalog = Catalog.Null;
        public Class itemClass = Class.Null;
        public Subclass ItemSubclass = Subclass.Null;
        
        [Header("Catalog Data")]
        private Dictionary<ObscuredString, ObscuredInt> itemPrices = new ();
        private Dictionary<ObscuredString, ObscuredString> customCatalogDataSafe = new ();
        private CatalogItem catalogItem;
        public enum Catalog
        {
            Weapons, Support, Backpack, Character, Shop, Setup, Null
        }
        
        public enum Class
        {
            //Weapons
            Firearm, Melee, Grenade, 
            
            //Support
            Drug, Ammo, Fuel, Upgrade,
            
            //Backpack
            Loot, 
            
            //Character
            Rig, Stuff, Scheme, Season,
            
            //Shop
            Pass, Car, Currency,
            
            //Setup
            Unit, Impact, Room, Crate,
            
            Null,
            
            Pack, Referral
        }
        
        public enum Subclass
        {
            //Weapons (Firearm, Melee, Grenade)
            Pistol, SMG, Shotgun, Autogun, Rifle, Heavy,
            Blunt, Cutting, Stabbing, Sawing, 
            Fire, Toxic, Blast, Electro,
            
            //Support (Health, Ammo, Fuel, Upgrade)
            Health, Stim,
            LightBullets, ShotgunBullets, RifleBullets, PiercingBullets,
            CarFuel, 
            WeaponUpgrade, UnitUpgrade, CarUpgrade,
            
            //Backpack (Loot)
            Common, Collectible, Food, Upgrades,
            
            //Character (Rig, Stuff, Scheme, Season)
            Apocalypse, Zombies, BossZombies, Gang, BattleRoyale,
            Rucksack, Beard, Headwear, Glasses, Trinkets,
            NPC, Player, 
            Anarchy,
            
            //Shop (Pass, Skin, Currency)
            Gold, 

            //Setup (Unit, Impact, Room)
            Null, 
            
            //Extra Weapons (Firearm)
            Piercing
        }

        public enum Currency
        {
            GL, BC
        }
        
        public enum DataType
        {
            StatInt, StatFloat, StatString, FX, Material
        };

        #region Handle

        public Dictionary<string, string> GetUnsafeCustomData()
        {
            return DataHandler.ConvertToUnsafeData(customCatalogDataSafe);
        }

        public string GetTypedData(DataType dataType, string keyFromDeserializeDictionary)
        {
            var data = GetTypedData(dataType);
            if (data == null) return null;
            data.TryGetValue(keyFromDeserializeDictionary, out var value);
            return value;
        }

        public Dictionary<string, string> GetTypedData(DataType dataType)
        {
            GetUnsafeCustomData().TryGetValue(dataType.ToString(), out string value);
            
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            
            return DataHandler.Deserialize(value);
        }

        public string GetUnsafeValue(string key)
        {
            GetUnsafeCustomData().TryGetValue(key, out string value);
            
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            return value;
        }

        public CatalogItem GetCatalogItem()
        {
            return catalogItem;
        }

        public string GetMaterialName()
        {
            GetUnsafeCustomData().TryGetValue(DataType.Material.ToString(), out string value);
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Custom data value is empty");
                return null;
            }
            return value;
        }

        public ObscuredInt GetItemPrice(Currency currency = Currency.BC)
        {
            if (!itemPrices.ContainsKey(currency.ToString())) return 0;
            
            itemPrices.TryGetValue(currency.ToString(), out var value);
            return value;
        }
        

        #endregion
        
        #region Initialize

        public void SetSpriteItem(Sprite sprite)
        {
            itemSprite = sprite;
        }

        public void SetCatalogData(CatalogItem item)
        {
            catalogItem = item;
            name = item.DisplayName;

            SetSafeCatalogCustomData(item);
            SetSafeCatalogInfo(item);
        }
        
        
        public void SetSafeCatalogCustomData(CatalogItem itemData)
        {
            if (string.IsNullOrEmpty(itemData.CustomData)) return;
            
            var customDataUnsafe = DataHandler.Deserialize(itemData.CustomData);
            customCatalogDataSafe = DataHandler.ConvertToSafeData(customDataUnsafe);
        }

        public void SetSafeCatalogInfo(CatalogItem catalogItem)
        {
            itemName = catalogItem.DisplayName;
            itemId = catalogItem.ItemId;
            
            itemCatalog = KeyStore.GetCatalog(catalogItem.CatalogVersion);
            itemClass = KeyStore.GetClass(catalogItem.ItemClass);
            ItemSubclass = KeyStore.GetSubclass(GetFirstTag(catalogItem));
            
            SetPrice(catalogItem);
        }

        
        private void SetPrice(CatalogItem catalogItem)
        {
            if (catalogItem.VirtualCurrencyPrices != null)
            {
                itemPrices = DataHandler.ConvertToSafeData(catalogItem.VirtualCurrencyPrices);
            }
        }
        
        #endregion

        #region Get

        private string GetFirstTag(CatalogItem catalogItem)
        {
            if (catalogItem.Tags != null && catalogItem.Tags.Count != 0)
            {
                var itemTags = DataHandler.ConvertToSafeData(catalogItem.Tags);

                if (!string.IsNullOrEmpty(itemTags[0]))
                {
                    return itemTags[0];
                }
            }

            return null;
        }
        
        public int GetRequiredLevel()
        {
            var requiredLevelString = GetTypedData(DataType.StatInt, "RequiredLevel");
            
            if (!string.IsNullOrEmpty(requiredLevelString))
            {
                return int.Parse(requiredLevelString);
            }

            return 0;
        }
        
        public bool IsPremium()
        {
            var requiredPassString = GetTypedData(DataType.StatString, "Premium");
            
            if (!string.IsNullOrEmpty(requiredPassString))
            {
                return bool.Parse(requiredPassString);
            }

            return false;
        }
        
        
        public int GetItemChance()
        {
            var chanceString = GetTypedData(DataType.StatInt, "Chance");
            
            if (!string.IsNullOrEmpty(chanceString))
            {
                return int.Parse(chanceString);
            }

            return 0;
        }
        
        public Subclass GetBlockSubclass()
        {
            var value = GetTypedData(DataType.StatString, "BlockItem");
            return KeyStore.GetSubclass(value);
        }
        
        #endregion
    }
}
